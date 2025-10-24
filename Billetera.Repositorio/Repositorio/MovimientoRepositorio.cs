using Billetera.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billetera.BD.Datos;
using Microsoft.EntityFrameworkCore;
using BilleteraVirtual.Repositorio.Repositorios;
using Billetera.Shared.DTOS;

namespace Billetera.Repositorio.Repositorio
{
    public class MovimientoRepositorio : Repositorio<Movimiento>,IRepositorio<Movimiento>, IMovimientoRepositorio
    {
        private readonly AppDbContext context;

        public MovimientoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Movimiento> CrearMovimientoAsync(MovimientoCrearDto dto)
        {
            var cuenta = await context.TiposCuentas.FindAsync(dto.TipoCuentaId);
            if (cuenta == null)
            {
                throw new Exception("Cuenta no encontrada");
            }

            // Buscar tipo de movimiento
            var tipoMovimiento = await context.TipoMovimientos.FirstOrDefaultAsync(t => t.Id == dto.TipoMovimientoId);
            if (tipoMovimiento == null)
            {
                throw new Exception("Tipo de movimiento no encontrado");
            }
            // Buscar cuenta origen
            var cuentaOrigen = await context.TiposCuentas.FirstOrDefaultAsync(c => c.Id == dto.TipoCuentaId);
            if (cuentaOrigen == null)
            {
                throw new Exception("Cuenta origen no encontrada");
            }
            // Guardar saldo anterior
            var saldoAnterior = cuentaOrigen.Saldo;

            //Calcular nuevo saldo
            if (tipoMovimiento.Operacion == "suma")
            {
                cuentaOrigen.Saldo += dto.Monto;
            }
            else if (tipoMovimiento.Operacion == "resta")
            {
                if (cuentaOrigen.Saldo < dto.Monto)
                {
                    throw new Exception("Saldo insuficiente para realizar la operación");
                }
                cuentaOrigen.Saldo -= dto.Monto;
            }
            else if (tipoMovimiento.Operacion == "transferencia")
            {
                if (dto.CuentaDestinoId == null)
                {
                    throw new Exception("Cuenta destino no especificada para la transferencia");
                }
                var cuentaDestino = await context.TiposCuentas.FirstOrDefaultAsync(c => c.Id == dto.CuentaDestinoId);
                if (cuentaDestino == null)
                {
                    throw new Exception("Cuenta destino no encontrada");
                }
                if (cuentaOrigen.Saldo < dto.Monto)
                {
                    throw new Exception("Saldo insuficiente para realizar la transferencia");
                }


                    // Actualizar saldos

                    cuentaOrigen.Saldo -= dto.Monto;
                cuentaDestino.Saldo += dto.Monto;

                // Registar movimiento de salida (origen)

                var movimientoSalida = new Movimiento
                {
                    TipoCuentaId = cuentaOrigen.Id,
                   TipoMovimientoId = dto.TipoMovimientoId,
                    MonedaTipo = cuenta.Moneda_Tipo,
                    Monto = dto.Monto,
                    Descripcion = $"Transferencia enviada a cuenta #{cuentaDestino.Id}",
                    Fecha = DateTime.Now,
                    Saldo_Anterior = saldoAnterior,
                    Saldo_Nuevo = cuentaOrigen.Saldo
                };

                // Registar movimiento de entrada (destino)
                var movimientoEntrada = new Movimiento
                {
                    TipoCuentaId = cuentaDestino.Id,
                    TipoMovimientoId = dto.TipoMovimientoId,
                    MonedaTipo = cuenta.Moneda_Tipo,
                    Monto = dto.Monto,
                    Descripcion = $"Transferencia recibida de cuenta #{cuentaOrigen.Id}",
                    Fecha = DateTime.Now,
                    Saldo_Anterior = cuentaDestino.Saldo - dto.Monto,
                    Saldo_Nuevo = cuentaDestino.Saldo
                };

                context.Movimientos.AddRange(movimientoEntrada, movimientoSalida);
                context.TiposCuentas.UpdateRange(cuentaOrigen, cuentaDestino);
                await context.SaveChangesAsync();

                return movimientoSalida;

            }


            // Crear objeto Movimiento

            var movimiento = new Movimiento
            {
                TipoCuentaId = dto.TipoCuentaId,
                TipoMovimientoId = dto.TipoMovimientoId,
                Monto = dto.Monto,
                Descripcion = dto.Descripcion,
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnterior,
                Saldo_Nuevo = cuentaOrigen.Saldo
            };

            // Guardar cambios en la base de datos
            context.Movimientos.Add(movimiento);
            context.TiposCuentas.Update(cuenta);
            await context.SaveChangesAsync();
            return movimiento;

        }

        public async Task<Movimiento> CompraMonedaAsync(MovimientoCompraDTO dto)
        {
            // Buscar cuentas origen y destino específicas
            var cuentaOrigen = await context.TiposCuentas
                .Include(c => c.Moneda)
                .FirstOrDefaultAsync(c => c.Id == dto.CuentaOrigenId);

            var cuentaDestino = await context.TiposCuentas
                .Include(c => c.Moneda)
                .FirstOrDefaultAsync(c => c.Id == dto.CuentaDestinoId);

            if (cuentaOrigen == null || cuentaDestino == null)
                throw new Exception("Cuenta origen o destino no encontrada");

            if (cuentaOrigen.Moneda == null || cuentaDestino.Moneda == null)
                throw new Exception("Las monedas de las cuentas no están definidas.");

            if (cuentaOrigen.Saldo < dto.MontoOrigen)
                throw new Exception("Saldo insuficiente para realizar la compra.");

            // Traer TODAS las cuentas asociadas a comprador y vendedor
            var cuentasComprador = await context.TiposCuentas
                .Include(c => c.Moneda)
                .Where(c => c.CuentaId == cuentaOrigen.CuentaId)
                .ToListAsync();

            var cuentasVendedor = await context.TiposCuentas
                .Include(c => c.Moneda)
                .Where(c => c.CuentaId == cuentaDestino.CuentaId)
                .ToListAsync();

            // Buscar las cuentas equivalentes por tipo de moneda
            var cuentaCompradorDestino = cuentasComprador.FirstOrDefault(c => c.Id == cuentaDestino.Id);
            var cuentaVendedorOrigen = cuentasVendedor.FirstOrDefault(c => c.Id == cuentaOrigen.Id);

            // Buscar precios base y calcular montos
            decimal precioBaseOrigen = cuentaOrigen.Moneda.PrecioBase;
            decimal precioBaseDestino = cuentaDestino.Moneda.PrecioBase;

            decimal comision = cuentaOrigen.Moneda.ComisionPorcentaje / 100;
            decimal montoDestino = dto.MontoOrigen * (precioBaseOrigen / precioBaseDestino);
            montoDestino *= (1 - comision);

            // === ACTUALIZAR SALDOS ===

            // Comprador
            cuentaOrigen.Saldo -= dto.MontoOrigen;
            if (cuentaCompradorDestino != null)
                cuentaCompradorDestino.Saldo += montoDestino;

            // Vendedor
            cuentaDestino.Saldo -= montoDestino;
            if (cuentaVendedorOrigen != null)
                cuentaVendedorOrigen.Saldo += dto.MontoOrigen;

            // === MOVIMIENTOS ===
            var movimientos = new List<Movimiento>();

            movimientos.Add(new Movimiento
            {
                TipoCuentaId = cuentaOrigen.Id,
                TipoMovimientoId = 4,
                MonedaTipo = cuentaOrigen.Moneda_Tipo,
                Monto = dto.MontoOrigen,
                Descripcion = $"Compra de {montoDestino} {cuentaDestino.Moneda_Tipo} en cuenta #{cuentaDestino.Id}",
                Fecha = DateTime.Now,
                Saldo_Anterior = cuentaOrigen.Saldo + dto.MontoOrigen,
                Saldo_Nuevo = cuentaOrigen.Saldo
            });

            if (cuentaCompradorDestino != null)
            {
                movimientos.Add(new Movimiento
                {
                    TipoCuentaId = cuentaCompradorDestino.Id,
                    TipoMovimientoId = 4,
                    MonedaTipo = cuentaCompradorDestino.Moneda_Tipo,
                    Monto = montoDestino,
                    Descripcion = $"Compra recibida en {cuentaCompradorDestino.Moneda_Tipo}",
                    Fecha = DateTime.Now,
                    Saldo_Anterior = cuentaCompradorDestino.Saldo - montoDestino,
                    Saldo_Nuevo = cuentaCompradorDestino.Saldo
                });
            }

            movimientos.Add(new Movimiento
            {
                TipoCuentaId = cuentaDestino.Id,
                TipoMovimientoId = 4,
                MonedaTipo = cuentaDestino.Moneda_Tipo,
                Monto = montoDestino,
                Descripcion = $"Venta de {montoDestino} {cuentaDestino.Moneda_Tipo}",
                Fecha = DateTime.Now,
                Saldo_Anterior = cuentaDestino.Saldo + montoDestino,
                Saldo_Nuevo = cuentaDestino.Saldo
            });

            if (cuentaVendedorOrigen != null)
            {
                movimientos.Add(new Movimiento
                {
                    TipoCuentaId = cuentaVendedorOrigen.Id,
                    TipoMovimientoId = 4,
                    MonedaTipo = cuentaVendedorOrigen.Moneda_Tipo,
                    Monto = dto.MontoOrigen,
                    Descripcion = $"Venta recibida en {cuentaVendedorOrigen.Moneda_Tipo}",
                    Fecha = DateTime.Now,
                    Saldo_Anterior = cuentaVendedorOrigen.Saldo - dto.MontoOrigen,
                    Saldo_Nuevo = cuentaVendedorOrigen.Saldo
                });
            }

            // === GUARDAR ===
            context.Movimientos.AddRange(movimientos);
            context.TiposCuentas.UpdateRange(cuentaOrigen, cuentaDestino);

            if (cuentaCompradorDestino != null)
                context.TiposCuentas.Update(cuentaCompradorDestino);
            if (cuentaVendedorOrigen != null)
                context.TiposCuentas.Update(cuentaVendedorOrigen);

            await context.SaveChangesAsync();

            return movimientos.First();
        }

       


        // Obtener todos los movimientos con detalles de cuenta y tipo de movimiento
        public async Task<IEnumerable<Movimiento>> ObtenerMovimientos()
        {
            return await context.Movimientos
            .Include(m => m.TipoCuenta)
            .Include(m => m.TipoMovimiento)
            .ToListAsync();
        }

        // Obtener un movimiento por ID con detalles de cuenta y tipo de movimiento
        public async Task<Movimiento?> GetByIdAsync(int id)
        {
            return await context.Movimientos
            .Include(m => m.TipoCuenta)
            .Include(m => m.TipoMovimiento)
            .FirstOrDefaultAsync(m => m.Id == id);
        }


    }
}


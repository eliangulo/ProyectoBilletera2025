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

            // Calcular nuevo saldo según el tipo de operación
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

                // Registrar movimiento de salida (origen)
                var movimientoSalida = new Movimiento
                {
                    TipoCuentaId = cuentaOrigen.Id,
                    TipoMovimientoId = dto.TipoMovimientoId,
                    MonedaTipo = cuentaOrigen.Moneda_Tipo,
                    Monto = dto.Monto,
                    Descripcion = $"Transferencia enviada a cuenta #{cuentaDestino.Id}",
                    Fecha = DateTime.Now,
                    Saldo_Anterior = saldoAnterior,
                    Saldo_Nuevo = cuentaOrigen.Saldo
                };

                // Registrar movimiento de entrada (destino)
                var movimientoEntrada = new Movimiento
                {
                    TipoCuentaId = cuentaDestino.Id,
                    TipoMovimientoId = dto.TipoMovimientoId,
                    MonedaTipo = cuentaDestino.Moneda_Tipo,
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

            // Crear objeto Movimiento para operaciones simples (depósito/extracción)
            var movimiento = new Movimiento
            {
                TipoCuentaId = dto.TipoCuentaId,
                TipoMovimientoId = dto.TipoMovimientoId,
                MonedaTipo = cuentaOrigen.Moneda_Tipo,  // ✅ Usar cuentaOrigen
                Monto = dto.Monto,
                Descripcion = dto.Descripcion,
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnterior,
                Saldo_Nuevo = cuentaOrigen.Saldo
            };

            // Guardar cambios en la base de datos
            context.Movimientos.Add(movimiento);
            context.TiposCuentas.Update(cuentaOrigen);  // ✅ Actualizar cuentaOrigen
            await context.SaveChangesAsync();

            return movimiento;
        }

        public async Task<Movimiento> CompraMonedaAsync(MovimientoCompraDTO dto)
        {
            // Buscamos las cuentas
            var cuentaOrigen = await context.TiposCuentas
            .Include(c => c.Moneda)
            .FirstOrDefaultAsync(c => c.Id == dto.CuentaOrigenId);

            var cuentaDestino = await context.TiposCuentas
                .Include(c => c.Moneda)
                .FirstOrDefaultAsync(c => c.Id == dto.CuentaDestinoId);

            // Validamos por si hay algún error
            if (cuentaOrigen == null || cuentaDestino == null)
            {
                throw new Exception("Cuenta origen o destino no encontrada");
            }

            if (cuentaOrigen.Moneda == null || cuentaDestino.Moneda == null)
            {
                throw new Exception("Las monedas de las cuentas no están definidas.");
            }

            if (cuentaOrigen.Saldo < dto.MontoOrigen)
                throw new Exception("Saldo insuficiente para realizar la compra.");

            // Buscamos los precios base
            decimal precioBaseOrigen = cuentaOrigen.Moneda.PrecioBase;
            decimal precioBaseDestino = cuentaDestino.Moneda.PrecioBase;

            // Calculamos la comisión
            decimal comision = cuentaOrigen.Moneda.ComisionPorcentaje / 100;

            // Calculamos la conversión
            decimal montoDestino = dto.MontoOrigen * (precioBaseOrigen / precioBaseDestino);
            montoDestino *= (1 - comision); // Aplicar la comisión

            // Saldos anteriores
            var saldoAnteriorOrigen = cuentaOrigen.Saldo;
            var saldoAnteriorDestino = cuentaDestino.Saldo;

            // Actualizar saldos
            cuentaOrigen.Saldo -= dto.MontoOrigen;
            cuentaDestino.Saldo += montoDestino;

            // Crear movimientos
            var movimientoSalida = new Movimiento
            {
                TipoCuentaId = cuentaOrigen.Id,
                TipoMovimientoId = 4,
                MonedaTipo = cuentaOrigen.Moneda_Tipo,
                Monto = dto.MontoOrigen,
                Descripcion = $"Compra de {montoDestino} {cuentaDestino.Moneda_Tipo} en cuenta #{cuentaDestino.Id}",
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnteriorOrigen,
                Saldo_Nuevo = cuentaOrigen.Saldo
            };

            var movimientoEntrada = new Movimiento
            {
                TipoCuentaId = cuentaDestino.Id,
                TipoMovimientoId = 4,
                MonedaTipo = cuentaDestino.Moneda_Tipo,
                Monto = montoDestino,
                Descripcion = $"Compra desde {dto.MontoOrigen} {cuentaOrigen.Moneda_Tipo} en cuenta #{cuentaOrigen.Id}",
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnteriorDestino,
                Saldo_Nuevo = cuentaDestino.Saldo
            };

            // Guardar en la base de datos
            context.Movimientos.AddRange(movimientoEntrada, movimientoSalida);
            context.TiposCuentas.UpdateRange(cuentaOrigen, cuentaDestino);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error al guardar los cambios: {ex.InnerException?.Message ?? ex.Message}");
            }

            return movimientoEntrada;
        }



        // Obtener todos los movimientos con detalles de cuenta y tipo de movimiento
        public async Task<IEnumerable<Movimiento>> ObtenerMovimientos()
        {
            return await context.Movimientos
            .Include(m => m.TipoCuenta!)
            //Porque necesitamos acceder a Cuenta.BilleteraId para
            //filtrar los movimientos por billetera del usuario.
            //El .ThenInclude() nos permite cargar la relación anidada de TipoCuenta > Cuenta.
            .ThenInclude(tc => tc.Cuenta!)
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


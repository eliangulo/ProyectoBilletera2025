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
                if (cuentaOrigen.MonedaId != cuentaDestino.MonedaId)
                {
                    throw new Exception("No se puede transferir a diferentes monedas.");
                }

                // Actualizar saldos
                cuentaOrigen.Saldo -= dto.Monto;
                cuentaDestino.Saldo += dto.Monto;

                // Usar la descripción personalizada si existe, sino usar por defecto
                string descripcionSalida;
                string descripcionEntrada;

                if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                {
                    // Si hay descripción personalizada, usarla y agregar referencia a cuenta
                    descripcionSalida = $"{dto.Descripcion} a cuenta #{cuentaDestino.Id}";
                    descripcionEntrada = $"{dto.Descripcion} de cuenta #{cuentaOrigen.Id}";
                }
                else
                {
                    // Descripción por defecto
                    descripcionSalida = $"Transferencia enviada a cuenta #{cuentaDestino.Id}";
                    descripcionEntrada = $"Transferencia recibida de cuenta #{cuentaOrigen.Id}";
                }

                // Registrar movimiento de salida (origen)
                var movimientoSalida = new Movimiento
                {
                    TipoCuentaId = cuentaOrigen.Id,
                    TipoMovimientoId = dto.TipoMovimientoId,
                    MonedaTipo = cuentaOrigen.Moneda_Tipo,
                    Monto = dto.Monto,
                    Descripcion = descripcionSalida,
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
                    Descripcion = descripcionEntrada,
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
                MonedaTipo = cuentaOrigen.Moneda_Tipo,  // Usar cuentaOrigen
                Monto = dto.Monto,
                Descripcion = dto.Descripcion,
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnterior,
                Saldo_Nuevo = cuentaOrigen.Saldo
            };

            // Guardar cambios en la base de datos
            context.Movimientos.Add(movimiento);
            context.TiposCuentas.Update(cuentaOrigen);  // Actualizar cuentaOrigen
            await context.SaveChangesAsync();

            return movimiento;
        }

        public async Task<Movimiento> CompraMonedaAsync(MovimientoCompraDTO dto)
        {

            // ===== PASO 1: CARGAR AMBAS CUENTAS =====
            var cuentaOrigen = await context.TiposCuentas
                .Include(c => c.Moneda)
                .Include(c => c.Cuenta)
                .FirstOrDefaultAsync(c => c.Id == dto.CuentaOrigenId);

            var cuentaDestino = await context.TiposCuentas
                .Include(c => c.Moneda)
                .Include(c => c.Cuenta)
                .FirstOrDefaultAsync(c => c.Id == dto.CuentaDestinoId);

            // ===== PASO 2: VALIDACIONES =====
            if (cuentaOrigen == null || cuentaDestino == null)
            {
                throw new Exception("Una o más cuentas no fueron encontradas");
            }

            if (cuentaOrigen.Moneda == null || cuentaDestino.Moneda == null)
            {
                throw new Exception("Las monedas de las cuentas no están definidas");
            }

            // Validar que no sea la misma moneda
            if (cuentaOrigen.MonedaId == cuentaDestino.MonedaId)
            {
                throw new Exception("No puedes comprar la misma moneda. Para transferir entre cuentas de la misma moneda, usa Transferencia");
            }

            // Validar que ambas cuentas pertenezcan al mismo usuario
            if (cuentaOrigen.CuentaId != cuentaDestino.CuentaId)
            {
                throw new Exception("Ambas cuentas deben pertenecer al mismo usuario");
            }

            // ===== PASO 3: CÁLCULOS =====
            decimal precioBaseOrigen = cuentaOrigen.Moneda.PrecioBase;
            decimal precioBaseDestino = cuentaDestino.Moneda.PrecioBase;
            decimal comisionPorcentaje = cuentaOrigen.Moneda.ComisionPorcentaje / 100;

            // Calcular cuánto recibirá (con comisión restada)
            decimal montoDestino = dto.MontoOrigen * (precioBaseOrigen / precioBaseDestino);
            montoDestino *= (1 - comisionPorcentaje); // Aplicar comisión

            // ===== PASO 4: VALIDAR SALDOS =====
            if (cuentaOrigen.Saldo < dto.MontoOrigen)
            {
                throw new Exception($"Saldo insuficiente. Disponible: {cuentaOrigen.Saldo:N2}, Requerido: {dto.MontoOrigen:N2}");
            }

            // Validar límite de compra (10 millones)
            if (dto.MontoOrigen > 10000000)
            {
                throw new Exception("El monto máximo de compra es de $10,000,000");
            }

            // ===== PASO 5: GUARDAR SALDOS ANTERIORES =====
            var saldoAnteriorOrigen = cuentaOrigen.Saldo;
            var saldoAnteriorDestino = cuentaDestino.Saldo;

            // ===== PASO 6: ACTUALIZAR SALDOS =====
            cuentaOrigen.Saldo -= dto.MontoOrigen;
            cuentaDestino.Saldo += montoDestino;

            // ===== PASO 7: CREAR LOS 2 MOVIMIENTOS =====
            var movimientos = new List<Movimiento>();

            // Descripción personalizada o por defecto
            string descripcionBase = string.IsNullOrEmpty(dto.Descripcion)
                ? $"Compra de {cuentaDestino.Moneda.TipoMoneda} con {cuentaOrigen.Moneda.TipoMoneda}"
                : dto.Descripcion;

            // Movimiento 1: Salida de dinero (lo que paga)
            var movimientoSalida = new Movimiento
            {
                TipoCuentaId = cuentaOrigen.Id,
                TipoMovimientoId = 4, // ID de "Compra"
                MonedaTipo = cuentaOrigen.Moneda_Tipo,
                Monto = dto.MontoOrigen,
                Descripcion = $"{descripcionBase} (pagado)",
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnteriorOrigen,
                Saldo_Nuevo = cuentaOrigen.Saldo
            };

            // Movimiento 2: Entrada de moneda comprada
            var movimientoEntrada = new Movimiento
            {
                TipoCuentaId = cuentaDestino.Id,
                TipoMovimientoId = 4, // ID de "Compra"
                MonedaTipo = cuentaDestino.Moneda_Tipo,
                Monto = montoDestino,
                Descripcion = $"{descripcionBase} (recibido con {comisionPorcentaje * 100}% comisión)",
                Fecha = DateTime.Now,
                Saldo_Anterior = saldoAnteriorDestino,
                Saldo_Nuevo = cuentaDestino.Saldo
            };

            movimientos.Add(movimientoSalida);
            movimientos.Add(movimientoEntrada);

            // ===== PASO 8: GUARDAR EN BASE DE DATOS =====
            context.Movimientos.AddRange(movimientos);
            context.TiposCuentas.UpdateRange(cuentaOrigen, cuentaDestino);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error al guardar la transacción: {ex.InnerException?.Message ?? ex.Message}");
            }

            return movimientoEntrada; // Retornar el movimiento de entrada como confirmación
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
            .ThenInclude(c => c.Billetera!)
            .ThenInclude(b => b.Usuarios) // Para obtener nombre del usuario
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


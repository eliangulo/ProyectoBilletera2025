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


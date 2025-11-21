using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/Movimiento")]
    public class MovimientoController : ControllerBase
    {
        private readonly IMovimientoRepositorio rep;
        private readonly IRepositorio<Movimiento> repositorio;

        public MovimientoController(IRepositorio<Movimiento> repositorio, IMovimientoRepositorio rep)
        {
            this.repositorio = repositorio;
            this.rep = rep;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovimientoDTO>>> GetMovimientos()
        {
            var movimientos = await rep.ObtenerMovimientos();

            var dtos = movimientos.Select(m => new MovimientoDTO
            {
                Id = m.Id,
                TipoCuentaId = m.TipoCuentaId,
                TipoMovimientoNombre = m.TipoMovimiento!.Nombre,
                MonedaTipo = m.MonedaTipo,
                Monto = m.Monto,
                Descripcion = m.Descripcion,
                Fecha = m.Fecha,
                Saldo_Anterior = m.Saldo_Anterior,
                Saldo_Nuevo = m.Saldo_Nuevo
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<MovimientoDTO>> GetById(int Id)
        {
            var movimiento = await rep.GetByIdAsync(Id);
            if (movimiento == null)
            {
                return NotFound($"Movimiento con Id {Id} no encontrado.");
            }
            return Ok(movimiento);
        }

        [HttpGet("billetera/{billeteraId}")]
        public async Task<ActionResult<List<MovimientoDTO>>> GetMovimientosByBilleteraId(int billeteraId)
        {
            try
            {
                var movimientos = await rep.ObtenerMovimientos();

                // Filtrar solo los movimientos de las cuentas de esta billetera
                var movimientosFiltrados = movimientos
                    .Where(m => m.TipoCuenta != null &&
                               m.TipoCuenta.Cuenta != null &&
                               m.TipoCuenta.Cuenta.BilleteraId == billeteraId)
                    .Select(m => 
                    {
                        var dto = new MovimientoDTO
                        {
                            Id = m.Id,
                            TipoCuentaId = m.TipoCuentaId,
                            TipoMovimientoId = m.TipoMovimientoId,
                            TipoMovimientoNombre = m.TipoMovimiento?.Nombre,
                            MonedaTipo = m.MonedaTipo,
                            Monto = m.Monto,
                            Fecha = m.Fecha,
                            Saldo_Anterior = m.Saldo_Anterior,
                            Saldo_Nuevo = m.Saldo_Nuevo,
                            Descripcion = m.Descripcion
                        };

                        //Si es transferencia o compra, buscar info del remitente
                        if (m.TipoMovimiento?.Nombre == "Transferencia" || m.TipoMovimiento?.Nombre == "Compra")
                        {
                            // Extraer el ID de la descripción: "cuenta #2" o "cuenta #123"
                            var match = System.Text.RegularExpressions.Regex.Match(
                                m.Descripcion, @"cuenta #(\d+)");

                            if (match.Success && int.TryParse(match.Groups[1].Value, out int tipoCuentaId))
                            {
                                // Buscar la TipoCuenta relacionada
                                var tipoCuentaRelacionada = movimientos
                                    .FirstOrDefault(mov => mov.TipoCuentaId == tipoCuentaId)
                                    ?.TipoCuenta;

                                if (tipoCuentaRelacionada != null)
                                {
                                    var usuario = tipoCuentaRelacionada.Cuenta!.Billetera!.Usuarios!.FirstOrDefault();

                                    string nombreCompleto = $"{usuario!.Nombre} {usuario.Apellido}";
                                    dto.RemitenteNombre = nombreCompleto;
                                    dto.RemitenteAlias = tipoCuentaRelacionada.Alias;

                                    //Esta línea reemplaza "cuenta #X" por el nombre completo del remitente en la descripción
                                    //Resume la info en un solo campo, y no usamos tantos if como en codigo anterior
                                    dto.Descripcion = System.Text.RegularExpressions.Regex.Replace(
                                        m.Descripcion,
                                        @"cuenta #\d+",
                                        nombreCompleto
                                    );
                                }
                            }
                        }

                        return dto;
                    })
                    .OrderByDescending(m => m.Fecha)
                    .ToList();

                return Ok(movimientosFiltrados);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener movimientos: {ex.Message}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CrearMovimiento(MovimientoCrearDto dto)
        {
            try
            {
                var movimiento = await rep.CrearMovimientoAsync(dto);
                return Ok(new
                {
                    mensaje = "Movimiento registrado correctamente",
                    movimiento = new
                    {
                        id = movimiento.Id,
                        monto = movimiento.Monto,
                        saldoNuevo = movimiento.Saldo_Nuevo
                    }
                });
            }
            catch (Exception ex)
            {
                //Siempre devolver JSON
                return BadRequest(new { error = ex.Message });
            }

        }

        [HttpPost("ComprarMoneda")]
        public async Task<IActionResult> ComprarMoneda(MovimientoCompraDTO dto)
        {
            try
            {
                await rep.CompraMonedaAsync(dto);
                return Ok("Compra Realizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
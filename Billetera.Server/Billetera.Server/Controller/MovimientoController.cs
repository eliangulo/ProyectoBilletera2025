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
                TipoMovimientoNombre = m.TipoMovimiento.Nombre,
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

        [HttpPost]
        public async Task<IActionResult> CrearMovimiento(MovimientoCrearDto dto)
        {
            try
            {
                await rep.CrearMovimientoAsync(dto);
                return Ok("Movimiento registrado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
        }




    }
}
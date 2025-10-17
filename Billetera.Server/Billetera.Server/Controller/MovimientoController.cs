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
        private readonly IRepositorio<Movimiento> repositorio;

        public MovimientoController(IRepositorio<Movimiento> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovimientoDTO>>> GetMovimientos()
        {
            var entidades = await repositorio.Select();
            var dtos = entidades.Select(e => new MovimientoDTO
            {
                Monto = e.Monto,
                Descripcion = e.Descripcion,
                Fecha = e.Fecha,
                Saldo_Anterior = e.Saldo_Anterior,
                Saldo_Nuevo = e.Saldo_Nuevo
            }).ToList();
            return Ok(dtos);
        }
        [HttpGet("{Id:int}")]
        public async Task<ActionResult<MovimientoDTO>> GetById(int Id)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
                return NotFound($"Movimiento con Id {Id} no encontrada.");
            var dto = new MovimientoDTO
            {
                Monto = entidad.Monto,
                Descripcion = entidad.Descripcion,
                Fecha = entidad.Fecha,
                Saldo_Anterior = entidad.Saldo_Anterior,
                Saldo_Nuevo = entidad.Saldo_Nuevo
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<MovimientoDTO>> Create(MovimientoDTO dto)
        {
            try
            {
                var entidad = new Movimiento
                {
                    CuentaId = dto.CuentaId,
                    TipoMovimientoId = dto.TipoMovimientoId,
                    Monto = dto.Monto,
                    Descripcion = dto.Descripcion,
                    Fecha = dto.Fecha,
                    Saldo_Anterior = dto.Saldo_Anterior,
                    Saldo_Nuevo = dto.Saldo_Nuevo
                };
                var id = await repositorio.Insert(entidad);
                return Ok(entidad.Id);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

    }
}
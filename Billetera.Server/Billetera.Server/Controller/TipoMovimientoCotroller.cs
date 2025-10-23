using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;


namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/TipoMovimiento")]
    public class TipoMovimientoController : ControllerBase
    {
        private readonly IRepositorio<TipoMovimiento> repositorio;

        public TipoMovimientoController(IRepositorio<TipoMovimiento> repositorio)
        {
            this.repositorio = repositorio;
        }
        [HttpGet]
        public async Task<ActionResult<List<TipoMovimientoDTO>>> GetTipoMovimientos()
        {
            var entidades = await repositorio.Select();
            var dtos = entidades.Select(e => new TipoMovimientoDTO
            {
                TipoMovimientoId = e.Id,
                Nombre = e.Nombre,
                Operacion = e.Operacion,
                Descripcion = e.Descripcion
            }).ToList();
            return Ok(dtos);
        }
        [HttpPost]
        public async Task<ActionResult<TipoMovimientoDTO>> Create(TipoMovimientoDTO dto)
        {
            try
            {
                var entidad = new TipoMovimiento
                {
                    Nombre = dto.Nombre,
                    Operacion = dto.Operacion,
                    Descripcion = dto.Descripcion
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


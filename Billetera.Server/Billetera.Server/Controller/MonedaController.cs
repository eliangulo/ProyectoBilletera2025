using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/Moneda")]
    public class MonedaController : ControllerBase
    {
        private readonly IMonedaRepositorio repositorio;
        private readonly IRepositorio<Moneda> rep;
        public MonedaController(IMonedaRepositorio repositorio, IRepositorio<Moneda> rep)
        {
            this.repositorio = repositorio;
            this.rep = rep;
        }

        //Este Crud depende se hace con el codigo ISO de la moneda
        //para mi se usaria mas que el ID 

        [HttpGet]   //consulta todas las monedas 
        public async Task<ActionResult<List<MonedaIdDTO>>> GetMoneda()
        {
            var entidad = await rep.Select();
            var dtos = entidad.Select(e => new MonedaIdDTO
            {
                Id = e.Id,
                TipoMoneda = e.TipoMoneda,
                Habilitada = e.Habilitada,
                CodISO = e.CodISO

            }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{CodISO}")]   //consulta todas las monedas por el CodigoISO
        public async Task<ActionResult<MonedaIdDTO>> GetByCodigoISO(string CodISO)
        {
            var moneda = await repositorio.SelectByCodigoISO(CodISO);
            if (moneda == null)
            {
                return NotFound($"Moneda con codigo ISO  '{CodISO}'  no encontrada.");
            }
            var dto = new MonedaIdDTO
            {
                Id = moneda.Id,
                TipoMoneda = moneda.TipoMoneda,
                Habilitada = moneda.Habilitada,
                CodISO = moneda.CodISO
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<MonedaIdDTO>> Create(MonedaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest($"Datos no validos");
            }

            var existe = await repositorio.SelectByCodigoISO(dto.CodISO);
            if (existe != null)
            {
                return BadRequest($"Ya existe una moneda con el codigo ISO  '{dto.CodISO}'");
            }

            var entidad = new Moneda
            {
                TipoMoneda = dto.TipoMoneda,
                Habilitada = dto.Habilitada,
                CodISO = dto.CodISO
            };

            var id = await rep.Insert(entidad);
            var dep = new MonedaIdDTO
            {
                Id = id,
                TipoMoneda = dto.TipoMoneda,
                Habilitada = dto.Habilitada,
                CodISO = dto.CodISO

            };
            return CreatedAtAction(nameof(GetByCodigoISO), new { CodISO = dto.CodISO }, dep);
        }

        [HttpPut("{CodISO}")]
        public async Task<ActionResult> Update(string CodISO, MonedaDTO dto)
        {
            var entidad = await repositorio.SelectByCodigoISO(CodISO);
            if (entidad == null)
            {
                return NotFound($"Moneda con codigo ISO  '{CodISO}'  no encontrada.");
            }

            entidad.TipoMoneda = dto.TipoMoneda;
            entidad.Habilitada = dto.Habilitada;
            entidad.CodISO = dto.CodISO;

            var actualizado = await rep.Update(entidad.Id, entidad);
            if (!actualizado)
            {
                return BadRequest("No se pudo actualizar la moneda.");
            }

            return Ok("Moneda actualizada correctamente.");
        }

        [HttpDelete("{CodISO}")]
        public async Task<ActionResult> Delete(string CodISO)
        {
            var entidad = await repositorio.SelectByCodigoISO(CodISO);
            if (entidad == null)
            {
                return NotFound($"Moneda con codigo ISO  '{CodISO}'  no encontrada.");
            }

            var eliminado = await rep.Delete(entidad.Id);
            if (!eliminado)
            {
                return NotFound($"Moneda con id  '{CodISO}'  no se pudo eliminar correctamente.");
            }

            return Ok("Moneda eliminada");
        }

    }
}

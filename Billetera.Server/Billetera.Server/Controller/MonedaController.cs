using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/Monedas")] // Cambiado de "api/Moneda" a "api/Monedas"
    public class MonedaController : ControllerBase
    {
        private readonly IMonedaRepositorio repositorio;
        private readonly IRepositorio<Moneda> rep;

        public MonedaController(IMonedaRepositorio repositorio, IRepositorio<Moneda> rep)
        {
            this.repositorio = repositorio;
            this.rep = rep;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonedaIdDTO>>> GetMoneda()
        {
            try
            {
                var entidades = await rep.Select();

                var dtos = new List<MonedaIdDTO>();

                foreach (var e in entidades)
                {
                    dtos.Add(new MonedaIdDTO
                    {
                        Id = e.Id,
                        TipoMoneda = e.TipoMoneda,
                        Habilitada = e.Habilitada,
                        CodISO = e.CodISO,
                        PrecioBase = e.PrecioBase,
                        ComisionPorcentaje = e.ComisionPorcentaje,
                        FechaActualizacion = e.FechaActualizacion
                    });
                }

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener monedas: {ex.Message}");
            }
        }

        [HttpGet("{CodISO}")]
        public async Task<ActionResult<MonedaIdDTO>> GetByCodigoISO(string CodISO)
        {
            try
            {
                var moneda = await repositorio.SelectByCodigoISO(CodISO);
                if (moneda == null)
                {
                    return NotFound($"Moneda con codigo ISO '{CodISO}' no encontrada.");
                }

                var dto = new MonedaIdDTO
                {
                    Id = moneda.Id,
                    TipoMoneda = moneda.TipoMoneda,
                    Habilitada = moneda.Habilitada,
                    CodISO = moneda.CodISO,
                    PrecioBase = moneda.PrecioBase,
                    ComisionPorcentaje = moneda.ComisionPorcentaje,
                    FechaActualizacion = moneda.FechaActualizacion
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la moneda: {ex.Message}");
            }
        }

        [HttpGet("Activas")]
        public async Task<ActionResult<IEnumerable<MonedaIdDTO>>> GetMonedasActivas()
        {
            try
            {
                var entidades = await rep.Select();

                var dtos = new List<MonedaIdDTO>();

                foreach (var e in entidades.Where(x => x.Habilitada))
                {
                    dtos.Add(new MonedaIdDTO
                    {
                        Id = e.Id,
                        TipoMoneda = e.TipoMoneda,
                        Habilitada = e.Habilitada,
                        CodISO = e.CodISO,
                        PrecioBase = e.PrecioBase,
                        ComisionPorcentaje = e.ComisionPorcentaje,
                        FechaActualizacion = e.FechaActualizacion
                    });
                }

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener monedas activas: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MonedaIdDTO>> Create([FromBody] MonedaDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Datos no válidos");
                }

                if (string.IsNullOrWhiteSpace(dto.CodISO) || string.IsNullOrWhiteSpace(dto.TipoMoneda))
                {
                    return BadRequest("El código ISO y el nombre de la moneda son requeridos");
                }

                var existe = await repositorio.SelectByCodigoISO(dto.CodISO);
                if (existe != null)
                {
                    return BadRequest($"Ya existe una moneda con el codigo ISO '{dto.CodISO}'");
                }

                var entidad = new Moneda
                {
                    TipoMoneda = dto.TipoMoneda,
                    Habilitada = dto.Habilitada,
                    CodISO = dto.CodISO.ToUpper(),
                    PrecioBase = dto.PrecioBase,
                    ComisionPorcentaje = dto.ComisionPorcentaje,
                    FechaActualizacion = DateTime.Now
                };

                var id = await rep.Insert(entidad);

                var resultado = new MonedaIdDTO
                {
                    Id = id,
                    TipoMoneda = entidad.TipoMoneda,
                    Habilitada = entidad.Habilitada,
                    CodISO = entidad.CodISO,
                    PrecioBase = entidad.PrecioBase,
                    ComisionPorcentaje = entidad.ComisionPorcentaje,
                    FechaActualizacion = entidad.FechaActualizacion
                };

                return CreatedAtAction(nameof(GetByCodigoISO), new { CodISO = entidad.CodISO }, resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la moneda: {ex.Message}");
            }
        }

        [HttpPut("{CodISO}")]
        public async Task<ActionResult> Update(string CodISO, [FromBody] MonedaDTO dto)
        {
            try
            {
                var entidad = await repositorio.SelectByCodigoISO(CodISO);
                if (entidad == null)
                {
                    return NotFound($"Moneda con codigo ISO '{CodISO}' no encontrada.");
                }

                entidad.TipoMoneda = dto.TipoMoneda;
                entidad.Habilitada = dto.Habilitada;
                entidad.PrecioBase = dto.PrecioBase;
                entidad.ComisionPorcentaje = dto.ComisionPorcentaje;
                entidad.FechaActualizacion = DateTime.Now;

                var actualizado = await rep.Update(entidad.Id, entidad);
                if (!actualizado)
                {
                    return BadRequest("No se pudo actualizar la moneda.");
                }

                return Ok(new { message = "Moneda actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la moneda: {ex.Message}");
            }
        }

        [HttpDelete("{CodISO}")]
        public async Task<ActionResult> Delete(string CodISO)
        {
            try
            {
                var entidad = await repositorio.SelectByCodigoISO(CodISO);
                if (entidad == null)
                {
                    return NotFound($"Moneda con codigo ISO '{CodISO}' no encontrada.");
                }

                var eliminado = await rep.Delete(entidad.Id);
                if (!eliminado)
                {
                    return BadRequest($"Moneda con codigo ISO '{CodISO}' no se pudo eliminar correctamente.");
                }

                return Ok(new { message = "Moneda eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la moneda: {ex.Message}");
            }
        }

        [HttpPatch("{CodISO}/toggle")]
        public async Task<ActionResult> ToggleEstado(string CodISO)
        {
            try
            {
                var entidad = await repositorio.SelectByCodigoISO(CodISO);
                if (entidad == null)
                {
                    return NotFound($"Moneda con codigo ISO '{CodISO}' no encontrada.");
                }

                entidad.Habilitada = !entidad.Habilitada;
                entidad.FechaActualizacion = DateTime.Now;

                var actualizado = await rep.Update(entidad.Id, entidad);
                if (!actualizado)
                {
                    return BadRequest("No se pudo actualizar el estado de la moneda.");
                }

                return Ok(new
                {
                    message = $"Moneda {(entidad.Habilitada ? "habilitada" : "deshabilitada")} correctamente.",
                    habilitada = entidad.Habilitada
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cambiar el estado de la moneda: {ex.Message}");
            }
        }
    }
}
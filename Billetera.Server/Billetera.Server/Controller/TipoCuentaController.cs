using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/TipoCuenta")]
    public class TipoCuentaController : ControllerBase
    {
        private readonly IRepositorio<TipoCuenta> repositorio;

        public TipoCuentaController(IRepositorio<TipoCuenta> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoCuentaIdDTO>>> GetTiposCuentas()
        {
            var entidades = await repositorio.Select();
            var dtos = entidades.Select(e => new TipoCuentaIdDTO
            {
                Id = e.Id,
                TC_Nombre = e.TC_Nombre,
                Moneda_Tipo = e.Moneda_Tipo,
                MonedaId = e.MonedaId,
                Saldo = e.Saldo,
                CuentaId = e.CuentaId
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<TipoCuentaIdDTO>> GetById(int Id)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
            {
                return NotFound($"Tipo de cuenta con Id {Id} no encontrado.");
            }

            var dto = new TipoCuentaIdDTO
            {
                Id = entidad.Id,
                TC_Nombre = entidad.TC_Nombre,
                Moneda_Tipo = entidad.Moneda_Tipo,
                MonedaId = entidad.MonedaId,
                CuentaId = entidad.CuentaId
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<TipoCuentaIdDTO>> Create(TipoCuentaDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos no válidos.");

            var entidad = new TipoCuenta
            {
                TC_Nombre = dto.TC_Nombre,
                Moneda_Tipo = dto.Moneda_Tipo,
                MonedaId = dto.MonedaId,
                Saldo = dto.Saldo,
                CuentaId = dto.CuentaId
            };

            var id = await repositorio.Insert(entidad);

            var nuevoDTO = new TipoCuentaIdDTO
            {
                Id = id,
                TC_Nombre = dto.TC_Nombre,
                Moneda_Tipo = dto.Moneda_Tipo,
                MonedaId = dto.MonedaId,
                Saldo    = dto.Saldo,
                CuentaId = dto.CuentaId
            };

            return CreatedAtAction(nameof(GetById), new { Id = id }, nuevoDTO);
        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Update(int Id, TipoCuentaDTO dto)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
                return NotFound($"Tipo de cuenta con Id {Id} no encontrado.");

            entidad.TC_Nombre = dto.TC_Nombre;
            entidad.Moneda_Tipo = dto.Moneda_Tipo;
            entidad.MonedaId = dto.MonedaId;
            entidad.Saldo = dto.Saldo;
            entidad.CuentaId = dto.CuentaId;

            var actualizado = await repositorio.Update(Id, entidad);
            if (!actualizado)
                return BadRequest("No se pudo actualizar el tipo de cuenta.");

            return Ok("Tipo de cuenta actualizado correctamente.");
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await repositorio.Existe(Id);
            if (!existe)
                return NotFound($"Tipo de cuenta con Id {Id} no encontrado.");

            try
            {
                var eliminado = await repositorio.Delete(Id);
                if (!eliminado)
                    return BadRequest("No se pudo eliminar el tipo de cuenta.");

                return Ok("Tipo de cuenta eliminada correctamente.");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_Cuentas_TiposCuentas"))
                {
                    return BadRequest("No se puede eliminar este tipo de cuenta porque hay cuentas asociadas.");
                }

                return StatusCode(500, "Error interno al intentar eliminar el tipo de cuenta.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }
    }
}

using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/Cuenta")]
    public class CuentaController : ControllerBase
    {
        private readonly IRepositorio<Cuenta> repositorio;
        private readonly ICuentaRepositorio cuentaRepo;

        public CuentaController(IRepositorio<Cuenta> repositorio, ICuentaRepositorio cuentaRepo)
        {
            this.repositorio = repositorio;
            this.cuentaRepo = cuentaRepo;
        }

        [HttpGet]
        public async Task<ActionResult<List<CuentaDTO>>> GetCuentas()
        {
            var entidades = await repositorio.Select();
            var dtos = entidades.Select(e => new CuentaDTO
            {
                Id = e.Id,
                BilleteraId = e.BilleteraId,
                Saldo = e.Saldo,
                NumCuenta = e.NumCuenta,
                EsCuentaDemo = e.EsCuentaDemo,
                SaldoDisponible = e.SaldoDisponible

            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<CuentaDTO>> GetById(int Id)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
                return NotFound($"Cuenta con Id {Id} no encontrada.");

            var dto = new CuentaDTO
            {
                Id = entidad.Id,
                BilleteraId = entidad.BilleteraId,
                Saldo = entidad.Saldo,
                NumCuenta = entidad.NumCuenta,
                EsCuentaDemo = entidad.EsCuentaDemo,
                SaldoDisponible = entidad.SaldoDisponible
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<CuentaDTO>> Create(CuentaDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos no válidos.");

            string numCuentaGenerado = await GenerarNumCuenta(dto.BilleteraId);

            var existeCuenta = await cuentaRepo.BuscarPorBilleteraYTipo(dto.BilleteraId, numCuentaGenerado);
            if (existeCuenta != null)
            {
                return BadRequest($"Ya existe una cuenta para la billetera {dto.BilleteraId} con ese tipo de cuenta.");
            }

            var entidad = new Cuenta
            {
                BilleteraId = dto.BilleteraId,
                Saldo = dto.Saldo,
                NumCuenta = numCuentaGenerado,
                EsCuentaDemo = true
            };

            var id = await repositorio.Insert(entidad);
            dto.Id = id;
            dto.NumCuenta = numCuentaGenerado;
            dto.EsCuentaDemo = entidad.EsCuentaDemo;
            dto.SaldoDisponible = entidad.SaldoDisponible;

            return CreatedAtAction(nameof(GetById), new { Id = id }, dto);
        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Update(int Id, CuentaDTO dto)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
                return NotFound($"Cuenta con Id {Id} no encontrada.");

            entidad.BilleteraId = dto.BilleteraId;
            entidad.Saldo = dto.Saldo;
            entidad.NumCuenta = dto.NumCuenta;
            entidad.EsCuentaDemo = dto.EsCuentaDemo;

            var actualizado = await repositorio.Update(Id, entidad);
            if (!actualizado)
                return BadRequest("No se pudo actualizar la cuenta.");

            return Ok("Cuenta actualizada correctamente.");
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await repositorio.Existe(Id);
            if (!existe)
                return NotFound($"Cuenta con Id {Id} no encontrada.");

            var eliminado = await repositorio.Delete(Id);
            if (!eliminado)
                return BadRequest("No se pudo eliminar la cuenta.");

            return Ok("Cuenta eliminada correctamente.");
        }

        private async Task<string> GenerarNumCuenta(int billeteraId)
        {
            // Obtiene cuántas cuentas existen para esa billetera
            var cuentasExistentes = await cuentaRepo.Select();
            int contador = cuentasExistentes.Count(c => c.BilleteraId == billeteraId) + 1;

            // Formato: C-0001-B{billeteraId}
            string numCuenta = $"C-{contador:D4}-B{billeteraId}";
            return numCuenta;
        }
    }
}

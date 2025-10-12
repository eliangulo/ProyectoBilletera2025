using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Billetera.Repositorio.Repositorio;
using Billetera.BD.Datos.Entity;
using Billetera.Shared.DTOS;
using Billetera.BD.Datos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Billetera.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BilleterasController : ControllerBase
    {
        private readonly IBilleteraRepositorio _billeteraRepositorio;
        private readonly AppDbContext _context;

        public BilleterasController(IBilleteraRepositorio billeteraRepositorio, AppDbContext context)
        {
            _billeteraRepositorio = billeteraRepositorio;
            _context = context;
        }

       
        /// Obtiene todas las billeteras (solo admin)
    
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BilleteraResponseDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BilleteraResponseDTO>>> GetAllBilleteras()
        {
            var billeteras = await _billeteraRepositorio.GetAllAsync();
            var dtos = billeteras.Select(b => new BilleteraResponseDTO
            {
                Id = b.Id,
                FechaCreacion = b.FechaCreacion,
                BilleteraAdmin = b.Billera_Admin,
                UsuarioPropietario = "Ver usuario"
            }).ToList();

            return Ok(dtos);
        }


        /// Obtiene una billetera por ID
    
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BilleteraResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BilleteraResponseDTO>> GetBilleteraById(int id)
        {
            var billetera = await _billeteraRepositorio.GetByIdAsync(id);

            if (billetera == null)
                return NotFound(new { message = "Billetera no encontrada" });

            var dto = new BilleteraResponseDTO
            {
                Id = billetera.Id,
                FechaCreacion = billetera.FechaCreacion,
                BilleteraAdmin = billetera.Billera_Admin,
                UsuarioPropietario = "Ver usuario"
            };

            return Ok(dto);
        }


        /// Obtiene la billetera del usuario actual con todas sus cuentas
     
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(BilleteraConCuentasDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BilleteraConCuentasDTO>> GetBilleteraByUsuario(int usuarioId)
        {
            var billetera = await _billeteraRepositorio.GetByUsuarioIdAsync(usuarioId);

            if (billetera == null)
                return NotFound(new { message = "Billetera no encontrada para este usuario" });

            var cuentas = await _context.Cuentas
                .Where(c => c.BilleteraId == billetera.Id)
                .ToListAsync();

            var dto = new BilleteraConCuentasDTO
            {
                Id = billetera.Id,
                FechaCreacion = billetera.FechaCreacion,
                BilleteraAdmin = billetera.Billera_Admin,
                UsuarioPropietario = "Tu billetera",
                Cuentas = cuentas.Select(c => new CuentaDTO
                {
                    Id = c.Id,
                    BilleteraId = c.BilleteraId,
                    TipoCuentaId = c.TipoCuentaId,
                    NumCuenta = c.NumCuenta,
                    Saldo = c.Saldo
                }).ToList()
            };

            return Ok(dto);
        }


        /// Actualiza el estado admin de una billetera (solo admin)
       
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BilleteraResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BilleteraResponseDTO>> UpdateBilletera(int id, [FromBody] BilleteraUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var billetera = await _billeteraRepositorio.GetByIdAsync(id);

            if (billetera == null)
                return NotFound(new { message = "Billetera no encontrada" });

            billetera.Billera_Admin = updateDTO.BilleteraAdmin;

            var updated = await _billeteraRepositorio.UpdateAsync(billetera);

            var dto = new BilleteraResponseDTO
            {
                Id = updated.Id,
                FechaCreacion = updated.FechaCreacion,
                BilleteraAdmin = updated.Billera_Admin,
                UsuarioPropietario = "Ver usuario"
            };

            return Ok(dto);
        }

    
        /// Elimina una billetera (solo admin)
  
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBilletera(int id)
        {
            var success = await _billeteraRepositorio.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = "Billetera no encontrada" });

            return NoContent();
        }
    }
}
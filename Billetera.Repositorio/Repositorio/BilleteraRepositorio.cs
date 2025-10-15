using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billetera.Repositorio.Repositorio
{
    public interface IBilleteraRepositorio
    {
        Task<IEnumerable<Billeteras>> GetAllAsync();
        Task<Billeteras?> GetByIdAsync(int id);
        Task<Billeteras?> GetByUsuarioIdAsync(int usuarioId);
        Task<Billeteras> CreateAsync(Billeteras billetera);
        Task<Billeteras> UpdateAsync(Billeteras billetera);
        Task<bool> DeleteAsync(int id);
    }

    public class BilleteraRepositorio : IBilleteraRepositorio
    {
        private readonly AppDbContext _context;

        public BilleteraRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Billeteras>> GetAllAsync()
        {
            return await _context.Billeteras.ToListAsync();
        }

        public async Task<Billeteras?> GetByIdAsync(int id)
        {
            return await _context.Billeteras
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Billeteras?> GetByUsuarioIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
            if (usuario == null)
                return null;

            return await _context.Billeteras
                .FirstOrDefaultAsync(b => b.Id == usuario.BilleteraId);
        }

        public async Task<Billeteras> CreateAsync(Billeteras billetera)
        {
            billetera.FechaCreacion = DateTime.Now;
            _context.Billeteras.Add(billetera);
            await _context.SaveChangesAsync();
            return billetera;
        }

        public async Task<Billeteras> UpdateAsync(Billeteras billetera)
        {
            _context.Billeteras.Update(billetera);
            await _context.SaveChangesAsync();
            return billetera;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var billetera = await GetByIdAsync(id);
            if (billetera == null)
                return false;

            _context.Billeteras.Remove(billetera);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
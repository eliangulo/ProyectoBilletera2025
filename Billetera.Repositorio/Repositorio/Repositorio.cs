using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billetera.BD.Datos;

namespace BilleteraVirtual.Repositorio.Repositorios
{
    public class Repositorio<E> : IRepositorio<E> where E : class, IEntityBase
    {
        private readonly AppDbContext context;
        public Repositorio(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> Existe(int id)
        {
            bool existe = await context.Set<E>().AnyAsync(x => x.Id == id);
            return existe;
        }

        public async Task<List<E>> Select()
        {
            return await context.Set<E>().ToListAsync();
        }

        public async Task<E?> SelectById(int id)
        {
            return await context.Set<E>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Insert(E entity)
        {
            try
            {
                await context.Set<E>().AddAsync(entity);
                await context.SaveChangesAsync();
                return entity.Id;
            }
            catch (Exception) { throw; }

        }

        public async Task<bool> Update(int id, E entity)
        {
            if (id != entity.Id) return false;

            var existe = await Existe(id);
            if (!existe) return false;

            try
            {
                context.Set<E>().Update(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await SelectById(id);
            if (entity == null) return false;
            try
            {
                context.Set<E>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}

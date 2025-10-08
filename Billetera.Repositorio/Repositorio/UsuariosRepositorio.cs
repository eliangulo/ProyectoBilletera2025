using global::Billetera.BD.Datos.Entity;
using global::Billetera.BD.Datos;
using Microsoft.EntityFrameworkCore;

namespace Billetera.Repositorio.Repositorio
{
    /// <summary>
    /// Repositorio para gestionar operaciones de Usuarios
    /// </summary>
    public class UsuariosRepositorio : IUsuariosRepositorio<Usuarios>
    {
        private readonly AppDbContext context;

        public UsuariosRepositorio(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Usuarios> GetAll()
        {
            return context.Usuario.AsQueryable();
        }

        public async Task<Usuarios> GetById(long id)
        {
            return await context.Usuario
                .Include(u => u.Billetera)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuarios> GetByCorreo(string correo)
        {
            return await context.Usuario
                .FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task<Usuarios> GetByCUIL(long cuil)
        {
            return await context.Usuario
                .FirstOrDefaultAsync(u => u.CUIL == cuil);
        }

        public async Task<bool> ExisteCorreo(string correo)
        {
            return await context.Usuario
                .AnyAsync(u => u.Correo == correo);
        }

        public async Task<bool> ExisteCUIL(long cuil)
        {
            return await context.Usuario
                .AnyAsync(u => u.CUIL == cuil);
        }

        public async Task<long> Insert(Usuarios entity)
        {
            try
            {
                entity.FechaCreacion = DateTime.Now;
                await context.Usuario.AddAsync(entity);
                await context.SaveChangesAsync();
                return entity.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Update(long id, Usuarios entity)
        {
            try
            {
                var usuarioExistente = await GetById(id);
                if (usuarioExistente == null)
                    return false;

                usuarioExistente.Nombre = entity.Nombre;
                usuarioExistente.Apellido = entity.Apellido;
                usuarioExistente.Domicilio = entity.Domicilio;
                usuarioExistente.Telefono = entity.Telefono;
                usuarioExistente.FechaNacimiento = entity.FechaNacimiento;

                context.Usuario.Update(usuarioExistente);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                var usuario = await GetById(id);
                if (usuario == null)
                    return false;

                context.Usuario.Remove(usuario);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public interface IUsuariosRepositorio<T>
    {
        IQueryable<T> GetAll();
        Task<T> GetById(long id);
        Task<long> Insert(T entity);
        Task<bool> Update(long id, T entity);
        Task<bool> Delete(long id);
        Task<T> GetByCorreo(string correo);
        Task<T> GetByCUIL(long cuil);
        Task<bool> ExisteCorreo(string correo);
        Task<bool> ExisteCUIL(long cuil);
    }
}


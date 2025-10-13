using Billetera.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuarios> Usuario { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<TipoCuenta> TiposCuentas { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<Billeteras> Billetera { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
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
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<TipoCuenta> TiposCuentas { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<Billeteras> Billeteras { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<TipoMovimiento> TipoMovimientos { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Delete Behavior
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
            // CUIL como único
            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => u.CUIL)
                .IsUnique();
            // Correo como único
            modelBuilder.Entity<Usuarios>()
                 .HasIndex(u => u.Correo)
                 .IsUnique();

            // inicial de Tipos de Movimiento para cripto
            modelBuilder.Entity<TipoMovimiento>().HasData(
                new TipoMovimiento
                {
                    Id = 1,
                    Nombre = "Depósito",
                    Operacion = "suma",
                    Descripcion = "Ingreso de dinero a la cuenta"
                },
                new TipoMovimiento
                {
                    Id = 2,
                    Nombre = "Extraccion",
                    Operacion = "resta",
                    Descripcion = "Retiro de dinero de la cuenta"
                },
                new TipoMovimiento
                {
                    Id = 3,
                    Nombre = "Transferencia",
                    Operacion = "transferencia",
                    Descripcion = "Movimiento que representa una transferencia a otra cuenta."
                },
                new TipoMovimiento
                {
                    Id = 4,
                    Nombre = "Compra",
                    Operacion = "resta",
                    Descripcion = "Compra realizada de un tipo de moneda a usuario o billetera."
                }
      
            );

        }
    }
}
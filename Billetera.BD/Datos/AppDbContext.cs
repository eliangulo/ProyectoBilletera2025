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
        public DbSet<Cuentas> Cuenta { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<Billeteras> Billetera { get; set; }
       // public DbSet<Compra> Compras { get; set; }
       // public DbSet<Deposito> Depositos { get; set; }
       //public DbSet<Extraccion> Extracion { get; set; }
       // public DbSet<Transferencia> Transferencias { get; set; }
        

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aquí puedes configurar tus entidades y relaciones


        }
    }
}


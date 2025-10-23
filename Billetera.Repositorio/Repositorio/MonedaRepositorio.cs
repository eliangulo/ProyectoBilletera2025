using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Repositorio.Repositorio
{
    public class MonedaRepositorio : Repositorio<Moneda>,
                                     IRepositorio<Moneda>,
                                     IMonedaRepositorio
    {
        private readonly AppDbContext context;

        public MonedaRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Moneda?> SelectByCodigoISO(string codISO)
        {
            try
            {
                codISO = codISO.ToUpper(); // Normaliza entrada
                return await context.Monedas
                    .FirstOrDefaultAsync(x => x.CodISO.ToUpper() == codISO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar moneda por código ISO: {ex.Message}", ex);
            }
        }

    }
}

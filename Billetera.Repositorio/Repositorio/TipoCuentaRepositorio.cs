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
    public class TipoCuentaRepositorio : Repositorio<TipoCuenta>, ITipoCuentaRepositorio
    {
        private readonly AppDbContext context;

        public TipoCuentaRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TipoCuenta>> GetTiposCuentaPorBilletera(int billeteraId)
        {
            return await context.TiposCuentas
                .Include(tc => tc.Cuenta)
                .Include(tc => tc.Moneda)
                .Where(tc => tc.Cuenta != null && tc.Cuenta.BilleteraId == billeteraId)
                .ToListAsync();
        }
    }
}

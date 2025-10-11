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
    public class CuentaRepositorio : Repositorio<Cuenta>, ICuentaRepositorio
    {
        private readonly AppDbContext context;

        public CuentaRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Cuenta?> BuscarPorBilleteraYTipo(int billeteraId, int tipoCuentaId)
        {
            return await context.Cuentas
                .FirstOrDefaultAsync(x => x.BilleteraId == billeteraId && x.TipoCuentaId == tipoCuentaId);
        }
    }
}

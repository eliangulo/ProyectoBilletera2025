using Billetera.BD.Datos.Entity;
using BilleteraVirtual.Repositorio.Repositorios;

namespace Billetera.Repositorio.Repositorio
{
    public interface ITipoCuentaRepositorio : IRepositorio<TipoCuenta>
    {
        Task<List<TipoCuenta>> GetTiposCuentaPorBilletera(int billeteraId);
    }
}
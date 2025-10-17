using Billetera.BD.Datos.Entity;

namespace Billetera.Repositorio.Repositorio
{
    public interface ICuentaRepositorio
    {
        Task<Cuenta?> BuscarPorBilleteraYTipo(int billeteraId, string numcuenta);

        Task<List<Cuenta>> Select();
    }
}
using Billetera.BD.Datos.Entity;

namespace Billetera.Repositorio.Repositorio
{
    public interface IMonedaRepositorio
    {
        Task<Moneda?> SelectByCodigoISO(string codISO);
    }
}
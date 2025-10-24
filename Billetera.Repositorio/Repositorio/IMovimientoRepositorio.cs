using Billetera.BD.Datos.Entity;
using Billetera.Shared.DTOS;

namespace Billetera.Repositorio.Repositorio
{
    public interface IMovimientoRepositorio
    {
        Task<Movimiento> CompraMonedaAsync(MovimientoCompraDTO dto);
        Task<Movimiento> CrearMovimientoAsync(MovimientoCrearDto dto);
        Task<Movimiento?> GetByIdAsync(int id);
        Task<IEnumerable<Movimiento>> ObtenerMovimientos();
    }
}
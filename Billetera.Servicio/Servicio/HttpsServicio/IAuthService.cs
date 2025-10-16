using Billetera.Shared.DTOS;

namespace Billetera.Servicio.ServiciosHttp
{
    public interface IAuthService
    {
        Task<bool> EstaAutenticado();
        Task<UsuariosDTO?> GetUsuarioActual();
        Task<string?> GetRol();
        Task Login(UsuariosDTO.Login usuario);
        Task Logout();
        Task<string?> GetToken();
    }
}
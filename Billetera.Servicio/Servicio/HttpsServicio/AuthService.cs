using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Billetera.Shared.DTOS;
using Microsoft.JSInterop;

namespace Billetera.Servicio.ServiciosHttp
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private UsuariosDTO? _usuarioActual;
        private string? _token;
        private const string TOKEN_KEY = "authToken";
        private const string USER_KEY = "authUser";

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task Login(UsuariosDTO.Login usuario)
        {
            var json = JsonSerializer.Serialize(usuario);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            var respuesta = await _http.PostAsync("api/usuarios/login", contenido);

            if (!respuesta.IsSuccessStatusCode)
            {
                var errorMsg = await respuesta.Content.ReadAsStringAsync();
                throw new Exception($"Login falló: {errorMsg}");
            }

            var cuerpo = await respuesta.Content.ReadAsStringAsync();
            var datos = JsonSerializer.Deserialize<LoginRespuestaDTO>(cuerpo,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _usuarioActual = datos?.Usuario;
            _token = datos?.Token;

            if (_token != null)
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                // Guardar en sessionStorage
                await _js.InvokeVoidAsync("sessionStorage.setItem", TOKEN_KEY, _token);
                if (_usuarioActual != null)
                {
                    await _js.InvokeVoidAsync("sessionStorage.setItem", USER_KEY, JsonSerializer.Serialize(_usuarioActual));
                }
            }
        }

        public async Task Logout()
        {
            _usuarioActual = null;
            _token = null;
            _http.DefaultRequestHeaders.Authorization = null;

            await _js.InvokeVoidAsync("sessionStorage.removeItem", TOKEN_KEY);
            await _js.InvokeVoidAsync("sessionStorage.removeItem", USER_KEY);
        }

        public async Task<bool> EstaAutenticado()
        {
            if (_token != null) return true;

            // Intentar recuperar del sessionStorage
            try
            {
                _token = await _js.InvokeAsync<string?>("sessionStorage.getItem", TOKEN_KEY);

                if (_token != null)
                {
                    var userJson = await _js.InvokeAsync<string?>("sessionStorage.getItem", USER_KEY);
                    if (userJson != null)
                    {
                        _usuarioActual = JsonSerializer.Deserialize<UsuariosDTO>(userJson);
                    }
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }
            }
            catch
            {
                // Si falla, asumimos no autenticado
                return false;
            }

            return _token != null;
        }

        public async Task<UsuariosDTO?> GetUsuarioActual()
        {
            if (_usuarioActual != null) return _usuarioActual;

            try
            {
                var userJson = await _js.InvokeAsync<string?>("sessionStorage.getItem", USER_KEY);
                if (userJson != null)
                {
                    _usuarioActual = JsonSerializer.Deserialize<UsuariosDTO>(userJson);
                }
            }
            catch
            {
                return null;
            }

            return _usuarioActual;
        }

        public async Task<string?> GetToken()
        {
            if (_token != null) return _token;

            try
            {
                _token = await _js.InvokeAsync<string?>("sessionStorage.getItem", TOKEN_KEY);
            }
            catch
            {
                return null;
            }

            return _token;
        }
    }

    public class LoginRespuestaDTO
    {
        public string Token { get; set; } = string.Empty;
        public UsuariosDTO Usuario { get; set; } = new();
    }
}
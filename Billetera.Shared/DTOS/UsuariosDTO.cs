using System.ComponentModel.DataAnnotations;

namespace Billetera.Shared.DTOS
{
    public class UsuariosDTO
    {
        public int Id { get; set; }
        public int BilleteraId { get; set; }
        [Required(ErrorMessage = "El CUIL es requerido")]
        [RegularExpression(@"^\d{2}-\d{8}-\d{1}$", ErrorMessage = "El formato debe ser XX-XXXXXXXX-X")]
        public string CUIL { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Domicilio { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public bool EsAdmin { get; set; }

        //registroUsuarios
        public class Registro
        {
            public string? CUIL { get; set; }
            public string? Nombre { get; set; }
            public string? Apellido { get; set; }
            public string? Domicilio { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public string? Correo { get; set; }
            public string? Telefono { get; set; }
            public string? Password { get; set; }
            public string? ConfirmarPassword { get; set; }
            public int BilleteraId { get; set; }
        }

        public class Login
        {
            public string CUIL { get; set; } = string.Empty;
            public string? Correo { get; set; }
            public string? Password { get; set; }
        }
        public class LoginAdmin
        {
            [Required(ErrorMessage = "La contraseña es requerida")]
            public string Password { get; set; } = string.Empty;
        }
    }
}
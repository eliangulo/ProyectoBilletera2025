using System.ComponentModel.DataAnnotations;

namespace Billetera.Shared.DTOS
{
    public class UsuariosDTO
    {
        public int Id { get; set; }
        public int BilleteraId { get; set; }
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
            [Required(ErrorMessage = "El CUIL es obligatorio")]
            [RegularExpression(@"^\d{2}-\d{8}-\d{1}$", ErrorMessage = "El formato debe ser XX-XXXXXXXX-X")]
            public string? CUIL { get; set; }
            [Required(ErrorMessage = "El nombre es obligatorio")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
            [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras")]
            public string? Nombre { get; set; }
            [Required(ErrorMessage = "El apellido es obligatorio")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
            [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras")]
            public string? Apellido { get; set; }
            [Required(ErrorMessage = "El domicilio es obligatorio")]
            [StringLength(200, MinimumLength = 5, ErrorMessage = "El domicilio debe tener entre 5 y 200 caracteres")]
            public string? Domicilio { get; set; }
            [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
            [DataType(DataType.Date)]
            public DateTime FechaNacimiento { get; set; }
            [Required(ErrorMessage = "El correo es obligatorio")]
            [EmailAddress(ErrorMessage = "El correo no es válido")]
            [StringLength(100)]
            public string? Correo { get; set; }
            [Required(ErrorMessage = "El teléfono es obligatorio")]
            [RegularExpression(@"^\d{2,4}\s\d{2,4}-\d{4}$", ErrorMessage = "Formato de teléfono inválido")]
            public string? Telefono { get; set; }
            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                  ErrorMessage = "La contraseña debe contener al menos: una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&)")]
            public string? Password { get; set; }
            public string? ConfirmarPassword { get; set; }
            public int BilleteraId { get; set; }
        }

        public class Login
        {
            [Required(ErrorMessage = "El CUIL es requerido")]
            [RegularExpression(@"^\d{2}-\d{8}-\d{1}$", ErrorMessage = "El formato debe ser XX-XXXXXXXX-X")]
            public string CUIL { get; set; } = string.Empty;
            [Required(ErrorMessage = "El correo es requerido")]
            public string? Correo { get; set; }
            [Required(ErrorMessage = "La contraseña es requerida")]
            public string? Password { get; set; }
        }
        public class LoginAdmin
        {
            [Required(ErrorMessage = "La contraseña es requerida")]
            public string Password { get; set; } = string.Empty;
        }
    }
}
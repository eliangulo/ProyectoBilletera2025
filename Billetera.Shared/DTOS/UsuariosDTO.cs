using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    namespace Billetera.Shared.DTOs
    {
        /// <summary>
        /// Contiene todos los DTOs relacionados con Usuarios
        /// </summary>
        public class UsuariosDTO
        {
            // DTO General para transferir información completa del usuario
            public int Id { get; set; }
            public int BilleteraId { get; set; }
            public long CUIL { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Domicilio { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public string Correo { get; set; }
            public string Telefono { get; set; }
            public bool EsAdmin { get; set; }

            /// <summary>
            /// DTO para el registro de nuevos usuarios
            /// </summary>
            public class Registro
            {
                [Required(ErrorMessage = "El CUIL es requerido")]
                [CUILValidation(ErrorMessage = "El CUIL no es válido")]
                public long CUIL { get; set; }

                [Required(ErrorMessage = "El nombre es requerido")]
                [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
                public string Nombre { get; set; }

                [Required(ErrorMessage = "El apellido es requerido")]
                [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
                public string Apellido { get; set; }

                [Required(ErrorMessage = "El domicilio es requerido")]
                [StringLength(200, ErrorMessage = "El domicilio no puede exceder los 200 caracteres")]
                public string Domicilio { get; set; }

                [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
                [DataType(DataType.Date)]
                [EdadMinima(18, ErrorMessage = "Debe ser mayor de 18 años para registrarse")]
                public DateTime FechaNacimiento { get; set; }

                [Required(ErrorMessage = "El correo es requerido")]
                [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
                [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres")]
                public string Correo { get; set; }

                [Required(ErrorMessage = "El teléfono es requerido")]
                [Phone(ErrorMessage = "El formato del teléfono no es válido")]
                [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
                public string Telefono { get; set; }

                [Required(ErrorMessage = "La contraseña es requerida")]
                [PasswordValidation(ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial")]
                public string Password { get; set; }

                [Required(ErrorMessage = "Debe confirmar la contraseña")]
                [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
                public string ConfirmarPassword { get; set; }

                public bool EsAdmin { get; set; } = false;
            }

            /// <summary>
            /// DTO para el inicio de sesión de usuarios
            /// </summary>
            public class Login
            {
                [Required(ErrorMessage = "El CUIL es requerido")]
                public long CUIL { get; set; }

                [Required(ErrorMessage = "El correo es requerido")]
                [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
                public string Correo { get; set; }

                [Required(ErrorMessage = "La contraseña es requerida")]
                public string Password { get; set; }
            }
        }

        #region Validaciones Personalizadas

        /// <summary>
        /// Validación personalizada para CUIL argentino
        /// </summary>
        public class CUILValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                    return new ValidationResult("El CUIL es requerido");

                string cuil = value.ToString();

                // El CUIL debe tener 11 dígitos
                if (cuil.Length != 11 || !long.TryParse(cuil, out _))
                    return new ValidationResult("El CUIL debe tener 11 dígitos numéricos");

                // Validar dígito verificador
                if (!ValidarDigitoVerificador(cuil))
                    return new ValidationResult("El CUIL no es válido");

                return ValidationResult.Success;
            }

            private bool ValidarDigitoVerificador(string cuil)
            {
                int[] multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                int suma = 0;

                for (int i = 0; i < 10; i++)
                {
                    suma += int.Parse(cuil[i].ToString()) * multiplicadores[i];
                }

                int resto = suma % 11;
                int digitoVerificador = resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;

                return digitoVerificador == int.Parse(cuil[10].ToString());
            }
        }

        /// <summary>
        /// Validación personalizada para contraseñas seguras
        /// </summary>
        public class PasswordValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                    return new ValidationResult("La contraseña es requerida");

                string password = value.ToString();

                // Mínimo 8 caracteres
                if (password.Length < 8)
                    return new ValidationResult("La contraseña debe tener al menos 8 caracteres");

                // Al menos una mayúscula
                if (!Regex.IsMatch(password, @"[A-Z]"))
                    return new ValidationResult("La contraseña debe contener al menos una letra mayúscula");

                // Al menos una minúscula
                if (!Regex.IsMatch(password, @"[a-z]"))
                    return new ValidationResult("La contraseña debe contener al menos una letra minúscula");

                // Al menos un número
                if (!Regex.IsMatch(password, @"[0-9]"))
                    return new ValidationResult("La contraseña debe contener al menos un número");

                // Al menos un carácter especial
                if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]"))
                    return new ValidationResult("La contraseña debe contener al menos un carácter especial");

                return ValidationResult.Success;
            }
        }

        /// <summary>
        /// Validación para verificar edad mínima
        /// </summary>
        public class EdadMinimaAttribute : ValidationAttribute
        {
            private readonly int _edadMinima;

            public EdadMinimaAttribute(int edadMinima)
            {
                _edadMinima = edadMinima;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                    return new ValidationResult("La fecha de nacimiento es requerida");

                if (value is DateTime fechaNacimiento)
                {
                    int edad = DateTime.Today.Year - fechaNacimiento.Year;

                    if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad))
                        edad--;

                    if (edad < _edadMinima)
                        return new ValidationResult($"Debe tener al menos {_edadMinima} años");

                    return ValidationResult.Success;
                }

                return new ValidationResult("Formato de fecha inválido");
            }
        }

        #endregion
    }
}

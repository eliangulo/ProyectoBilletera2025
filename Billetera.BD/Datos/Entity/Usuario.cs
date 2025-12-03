using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.BD.Datos.Entity
{
    [Index(nameof(CUIL), Name = "Usuarios_CUIL_UQ", IsUnique = true)]
    [Index(nameof(Correo), Name = "Usuarios_Correo_UQ", IsUnique = true)]
    public class Usuarios : EntityBase
    {

        //Relacion con Billetera
        [Required(ErrorMessage = "La billetera id es requerido")]
        public required int BilleteraId { get; set; }
        public Billeteras? Billetera { get; set; }

        //
        [Required(ErrorMessage = "El CUIL es requerido")]
        [StringLength(13)] // XX-XXXXXXXX-X
        [RegularExpression(@"^\d{2}-\d{8}-\d{1}$", ErrorMessage = "Formato de CUIL inválido")]
        public required string CUIL { get; set; } = string.Empty;
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 2)]
        public required string Nombre { get; set; }
        [StringLength(100, MinimumLength = 2)]
        public required string Apellido { get; set; }
        [Required(ErrorMessage = "El Domicilio es requerido")]
        [StringLength(200)]
        public required string Domicilio { get; set; }
 
        [Required(ErrorMessage = "La fecha de nacimiento es requerido")]
        public required DateTime FechaNacimiento { get; set; }
        [Required(ErrorMessage = "El correo es requerido")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "El telefono es requerido")]
        [StringLength(20)]
        public required string Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MaxLength(500)]
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public bool EsAdmin { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

    }
}

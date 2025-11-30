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
        public required long CUIL { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es requerido")]
        public required string Apellido { get; set; }
        [Required(ErrorMessage = "El Domicilio es requerido")]
        public required string Domicilio { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerido")]
        public required DateTime FechaNacimiento { get; set; }
        [Required(ErrorMessage = "El correo es requerido")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "El telefono es requerido")]
        public required string Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MaxLength(500)]
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public bool EsAdmin { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.BD.Datos.Entity
{
    public class Billeteras : EntityBase
    {
        [Required(ErrorMessage = "La Fecha es requerido")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El rol billetera es requerido")]
        public required bool Billera_Admin { get; set; } = false;

        //Eliii, Le agrege este ICollection para la relacion uno a muchos con Usuarios
        //Y asi poder acceder a los usuarios de una billetera, y con esto puedo hacer lo del alias unico
        public ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
    }
}

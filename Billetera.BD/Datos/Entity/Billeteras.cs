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
        public required DateTime FechaCreacion { get; set; }

        [Required(ErrorMessage = "El rol billetera es requerido")]
        public required bool Billera_Admin { get; set; }
    }
}

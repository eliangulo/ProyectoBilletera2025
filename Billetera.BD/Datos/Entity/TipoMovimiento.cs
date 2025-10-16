using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.BD.Datos.Entity
{
    public class TipoMovimiento : EntityBase
    {
        [Required(ErrorMessage = "El Nombre del Tipo de Movimiento es requerido")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "La Operacion es requerida")]
        public required string Operacion { get; set; } // "Ingreso" o "Egreso"
        public string Descripcion { get; set; } = string.Empty;
    }
}

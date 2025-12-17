using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class MonedaDTO
    {
        public string TipoMoneda { get; set; } = "";
        public bool Habilitada { get; set; }
        public string CodISO { get; set; } = "";
        [Required(ErrorMessage = "El precio base es requerido")]
        public decimal PrecioBase { get; set; }

        [Required(ErrorMessage = "La comisión es requerida")]
        [Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
        public decimal ComisionPorcentaje { get; set; } = 5.00M;

        public DateTime FechaActualizacion { get; set; }
    }
}

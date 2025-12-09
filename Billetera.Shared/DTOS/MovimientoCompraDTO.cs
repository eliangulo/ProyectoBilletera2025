using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class MovimientoCompraDTO
    {
        [Required(ErrorMessage = "La cuenta origen es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta origen válida")]
        public int CuentaOrigenId { get; set; }

        [Required(ErrorMessage = "La cuenta destino es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta destino válida")]
        public int CuentaDestinoId { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, 10000000, ErrorMessage = "El monto debe estar entre 0.01 y 10,000,000")]
        public decimal MontoOrigen { get; set; }

        public string? Descripcion { get; set; } 
    }
}

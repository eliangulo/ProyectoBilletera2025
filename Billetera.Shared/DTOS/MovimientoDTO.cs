using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class MovimientoDTO
    {
        public int Id { get; set; }
        public int TipoCuentaId { get; set; }
        public int TipoMovimientoId { get; set; }
        public string MonedaTipo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Saldo_Anterior { get; set; }
        public decimal Saldo_Nuevo { get; set; }
        public string? TipoMovimientoNombre { get; set; }

        // Esto lo agrego porque se trata de la info del remitente
        public string? RemitenteNombre { get; set; }    // EJ: "Belen Tejeda"
        public string? RemitenteAlias { get; set; }     // "Belen.Tejeda.USD"
    }
    public class ComprarCriptoDTO
    {
        [Required(ErrorMessage = "La cuenta es requerida")]
        public int CuentaId { get; set; }

        [Required(ErrorMessage = "La moneda es requerida")]
        public int MonedaId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadMoneda { get; set; }
    }
}

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
        public int CuentaId { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Saldo_Anterior { get; set; }
        public decimal Saldo_Nuevo { get; set; }
        public int? MonedaId { get; set; }
        public string? MonedaCodigo { get; set; }
        public string? MonedaNombre { get; set; }
        public decimal? CantidadMoneda { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? ComisionPorcentaje { get; set; }
        public decimal? ComisionMonto { get; set; }
        public string? TipoMovimientoNombre { get; set; }
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

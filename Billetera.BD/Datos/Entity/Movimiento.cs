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
    public class Movimiento : EntityBase
    {
        public required int TipoCuentaId { get; set; }
        public TipoCuenta? TipoCuenta { get; set; }
        public required int TipoMovimientoId { get; set; }
        public TipoMovimiento? TipoMovimiento { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "El monto es requerido")]
        public required decimal Monto { get; set; }
        public string MonedaTipo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;
        public required DateTime Fecha { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "El saldo anterior es requerido")]
        public required decimal Saldo_Anterior { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "El saldo nuevo es requerido")]
        public required decimal Saldo_Nuevo { get; set; }
    }
}

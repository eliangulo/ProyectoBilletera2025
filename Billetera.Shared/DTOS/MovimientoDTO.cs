using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class MovimientoDTO
    {
        public int CuentaId { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Saldo_Anterior { get; set; }
        public decimal Saldo_Nuevo { get; set; }
    }
}

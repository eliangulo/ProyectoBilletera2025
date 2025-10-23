using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class MovimientoCrearDto
    {
        public int TipoCuentaId { get; set; }
        public int TipoMovimientoId { get; set; }
        //public string NombreTipoMovimiento { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int? CuentaDestinoId { get; set; } // Para transferencias
    
    }
}

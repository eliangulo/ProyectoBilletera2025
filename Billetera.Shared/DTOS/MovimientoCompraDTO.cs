using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class MovimientoCompraDTO
    {
        public int CuentaOrigenId { get; set; } // ID de la cuenta con la moneda que usás para pagar

        public int CuentaDestinoId { get; set; } // ID de la cuenta donde recibís la moneda comprada
        public decimal MontoOrigen { get; set; } // Monto en la moneda origen
        public string? Descripcion { get; set; }

    }
}

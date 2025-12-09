using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class TipoCuentaDTO
    {
        public string TC_Nombre { get; set; } = "Caja de Ahorro";
        public string Moneda_Tipo { get; set; } = "";
        public int MonedaId { get; set; }
        public decimal Saldo { get; set; } = 0;
        public bool EsCuentaDemo { get; set; } = true;
        public int CuentaId { get; set; }
        public decimal SaldoDisponible { get; set; }
        public string Alias { get; set; } = "";
    }
}

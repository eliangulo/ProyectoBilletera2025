using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class TipoCuentaDTO
    {
        public string TC_Nombre { get; set; } = "";
        public string Moneda_Tipo { get; set; } = "";
        public int MonedaId { get; set; }
        public int CuentaId { get; set; }
    }
}

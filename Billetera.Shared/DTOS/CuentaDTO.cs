using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class CuentaDTO
    {
        public int Id { get; set; }
        public int BilleteraId { get; set; }
        public int TipoCuentaId { get; set; }
        public string NumCuenta { get; set; } = "";
        public decimal Saldo { get; set; }
    }
}

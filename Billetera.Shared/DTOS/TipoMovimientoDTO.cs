using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.Shared.DTOS
{
    public class TipoMovimientoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Operacion { get; set; } = string.Empty; // "Ingreso" o "Egreso"
        public string Descripcion { get; set; } = string.Empty;
    }
}

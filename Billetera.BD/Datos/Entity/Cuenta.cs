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
    //Este index es para que billetera solo tenga una cuenta
    [Index(nameof(BilleteraId), Name = "Cuenta_Billetera_UQ", IsUnique = true)]

    public class Cuenta : EntityBase
    {
        // Clave foranea
        public required int BilleteraId { get; set; }
        public Billeteras? Billetera { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Saldo { get; set; }

        [Required(ErrorMessage = "La Numero de cuenta es requerido")]
        public required string NumCuenta { get; set; }
        public ICollection<TipoCuenta>? TiposCuentas { get; set; }

        //public required int TipoCuentaId { get; set; }
        // public TipoCuenta? TiposCuentas { get; set; }

    }
}

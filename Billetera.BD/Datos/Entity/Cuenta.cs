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
   // [Index(nameof(Moneda_Tipo), Name = "Cuenta_Moneda_Tipo_UQ", IsUnique = true)]
    public class Cuentas : EntityBase
    {

        // Clave foranea
        public required int BilleteraId { get; set; }
        public Billeteras? Billetera { get; set; }

        [Required(ErrorMessage = "La moneda id es requerido")]
        public required int MonedaId { get; set; }
        public Moneda? Moneda { get; set; }


        [Required(ErrorMessage = "Debe ingresar el tipo moneda")]
        public required string Moneda_Tipo { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Saldo { get; set; }


    }
}

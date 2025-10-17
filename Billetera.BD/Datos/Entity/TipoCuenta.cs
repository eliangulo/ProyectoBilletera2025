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
    public class TipoCuenta : EntityBase
    {
        public Moneda? Moneda { get; set; }

        [Required(ErrorMessage = "La cuenta Id es requerida")]
        public required int CuentaId { get; set; }
        public Cuenta? Cuenta { get; set; }

        //

        [Required(ErrorMessage = "El Tipo de Cuenta es requerido")]
        [MaxLength(20, ErrorMessage = "El Tipo de Cuenta tiene como maximo 20 caracteres")]
        public required string TC_Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar el tipo moneda")]
        public required string Moneda_Tipo { get; set; }

        [Required(ErrorMessage = "La moneda id es requerido")]
        public required int MonedaId { get; set; }
        
    }
}

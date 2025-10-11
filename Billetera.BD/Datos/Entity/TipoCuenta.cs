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
        /* 
        Tengo que hacer la tabla TipoCuenta, en la cual puede pedir los siguientes datos
        Ahorro, Corriente, Cripto y no se que otras mas existin, menos idea yo
        y en que moneda va a usar la cuenta. 
        Entonces la tabla TipoCuents para debe de tener el Id, Tipo o Nombre, Moneda_Tipo
        */

        [Required(ErrorMessage = "El Tipo de Cuenta es requerido")]
        [MaxLength(20, ErrorMessage = "El Tipo de Cuenta tiene como maximo 20 caracteres")]
        public required string TC_Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar el tipo moneda")]
        public required string Moneda_Tipo { get; set; }

        [Required(ErrorMessage = "La moneda id es requerido")]
        public required int MonedaId { get; set; }
        public Moneda? Moneda { get; set; }
    }
}

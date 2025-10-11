using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.BD.Datos.Entity
{
    [Index(nameof(CodISO), Name = "Moneda_CodISO_UQ", IsUnique = true)]
    public class Moneda : EntityBase
    {
        //Tipo de moneda, es decir si es Pesos, Dolares, Euros, etc.
        [Required(ErrorMessage = "El Tipo de Moneda es requerido")]

        public required string TipoMoneda { get; set; }


        // Habilitada le puse un boolean para que se pueda habilitar o deshabilitar,
        //No se si les parece bien.

        [Required(ErrorMessage = "La Habilitacion de la moneda es requerido")]
        public required bool Habilitada { get; set; }

        // Codigo ISO de la moneda, ejemplo 32 para pesos argentinos, 840 para dolares, 978 para euros, etc.
        //pero lo dejo como string para que se use para las abreviaturas, es decir ARS, USD, EUR, etc.
        [Required(ErrorMessage = "El Codigo ISO de la moneda es requerido")]
        [MaxLength(3, ErrorMessage = "El Codigo ISO de la moneda  tiene como maximo {3} caracteres")]
        public required string CodISO { get; set; }

    }
}

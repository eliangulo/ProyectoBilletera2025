using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        //agrege para el panel Admin
        [Required(ErrorMessage = "El precio base es requerido")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioBase { get; set; } // Precio de cotización en tu moneda local

        [Required(ErrorMessage = "La comisión es requerida")]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
        public decimal ComisionPorcentaje { get; set; } = 5.00M; // Comisión por defecto 5%

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

    }
}

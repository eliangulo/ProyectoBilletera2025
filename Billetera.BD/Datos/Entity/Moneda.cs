using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billetera.BD.Datos.Entity
{
    public class Moneda : EntityBase
    {
        [Index(nameof(CodISO), Name = "Moneda_CodISO_UQ", IsUnique = true)]
        public class Monedas : EntityBase
        {

            [Required(ErrorMessage = "El Tipo de Moneda es requerido")]
            [MaxLength(3, ErrorMessage = "El Tipo de Moneda tiene como maximo {3} caracteres")]
            public required string TipoMoneda { get; set; }

            // Habilitada le puse un boolean para que se pueda habilitar o deshabilitar,
            //No se si les parece bien.

            [Required(ErrorMessage = "La Habilitacion de la moneda es requerido")]
            public required bool Habilitada { get; set; }

            //
            [Required(ErrorMessage = "El Codigo ISO es requerido para la representacion de la Moneda")]
            public int CodISO { get; set; }

        }
    }
}

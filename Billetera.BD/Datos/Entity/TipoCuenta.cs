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
    [Index(nameof(Alias), Name = "TipoCuenta_Alias_UQ", IsUnique = true)]
    public class TipoCuenta : EntityBase
    {
        [Required(ErrorMessage = "La moneda id es requerido")]
        public required int MonedaId { get; set; }
        public Moneda? Moneda { get; set; }

        [Required(ErrorMessage = "La cuenta Id es requerida")]
        public required int CuentaId { get; set; }
        public Cuenta? Cuenta { get; set; }

        //

        [Required(ErrorMessage = "El Tipo de Cuenta es requerido")]
        [MaxLength(20, ErrorMessage = "El Tipo de Cuenta tiene como maximo 20 caracteres")]
        public required string TC_Nombre { get; set; }

        //Alias único para esta cuenta específica
        [Required(ErrorMessage = "El alias es requerido")]
        [MaxLength(100)]
        public string Alias { get; set; } = ""; // "Ej: Eli.Lu.Pesos"

        [Required(ErrorMessage = "Debe ingresar el tipo moneda")]
        public required string Moneda_Tipo { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Saldo { get; set; }

        public bool EsCuentaDemo { get; set; } = true;
        [NotMapped] // No se guarda en la base de datos
        public decimal SaldoDisponible
        {
            get
            {
                if (EsCuentaDemo)
                    return 1000000.00M; // 1 millón (saldo infinito)
                return Saldo; // Saldo real
            }
        }

    }
}

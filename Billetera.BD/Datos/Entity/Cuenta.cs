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
    //anterior index; en este caso cuenta solo puede tener un solo tipo de moneda

    [Index(nameof(BilleteraId), nameof(TipoCuentaId), Name = "Cuenta_Billetera_Tipo_UQ", IsUnique = true)]
    //Nuevo index; en este caso una billetera solo puede tener un solo tipo de cuenta
    //y una cuenta solo puede pertenecer a una billetera
    public class Cuenta : EntityBase
    {
        // Clave foranea
        public required int BilleteraId { get; set; }
        public Billeteras? Billetera { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Saldo { get; set; }

        [Required(ErrorMessage = "La Numero de cuenta es requerido")]
        public required string NumCuenta { get; set; }

        public required int TipoCuentaId { get; set; }
        public TipoCuenta? TiposCuentas { get; set; }

        /*
         En la Tabla Cuenta tengo que modificarla, tengo que sacarle el MonedaTipo y MonedaId ya que tendria que ir en TipoCuenta
         y agregarle el TipoCuentaId que es la clave foranea de la tabla TipoCuenta. Por lo que nomas se queda el Saldo y la BilleteraId
        */
    }
}

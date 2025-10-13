using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Billetera.Shared.DTOS
{
    public class BilleteraDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La Fecha de Creación es requerida")]
        public DateTime FechaCreacion { get; set; }

        [Required(ErrorMessage = "El rol billetera es requerido")]
        public bool BilleteraAdmin { get; set; }

        public int UsuarioId { get; set; }
    }

    public class BilleteraUpdateDTO
    {
        [Required(ErrorMessage = "El rol billetera es requerido")]
        public bool BilleteraAdmin { get; set; }
    }

    public class BilleteraResponseDTO
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool BilleteraAdmin { get; set; }
        public string UsuarioPropietario { get; set; }
    }

    public class BilleteraConCuentasDTO
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool BilleteraAdmin { get; set; }
        public string UsuarioPropietario { get; set; }
        public List<CuentaDTO> Cuentas { get; set; } = new();
    }
}
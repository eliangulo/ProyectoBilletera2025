namespace Billetera.Shared.DTOS
{
    public class UsuariosDTO
    {
        public int Id { get; set; }
        public int BilleteraId { get; set; }
        public long CUIL { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Domicilio { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public bool EsAdmin { get; set; }

        //registroUsuarios
        public class Registro
        {
            public long CUIL { get; set; }
            public string? Nombre { get; set; }
            public string? Apellido { get; set; }
            public string? Domicilio { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public string? Correo { get; set; }
            public string? Telefono { get; set; }
            public string? Password { get; set; }
            public string? ConfirmarPassword { get; set; }
            public int BilleteraId { get; set; }
        }

        public class Login
        {
            public long CUIL { get; set; }
            public string? Correo { get; set; }
            public string? Password { get; set; }
        }
        public class LoginAdmin
        {
            public string? Password { get; set; }
        }
    }
}
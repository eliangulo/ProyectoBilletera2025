using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using Billetera.Shared.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Billetera.Repositorio.Repositorio;
using Billetera.Servicios;

namespace Billetera.Server.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosRepositorio<Usuarios> repositorio;
        private readonly AppDbContext context;
        private readonly IEncryptionService _encryptionService;

        public UsuariosController(AppDbContext context, IUsuariosRepositorio<Usuarios> repositorio, IEncryptionService encryptionService)
        {
            this.repositorio = repositorio;
            this.context = context;
            this._encryptionService = encryptionService;
        }

       
        /// Registra un nuevo usuario 
       
        
        [HttpPost("registro")]
        public async Task<ActionResult> RegistrarUsuario([FromBody] UsuariosDTO.Registro dto)
        {
            try
            {
                //  Validar duplicados correo y cuil
                if (await repositorio.ExisteCUIL(dto.CUIL))
                    return BadRequest("Este CUIL ya se encuentra registrado en el sistema");


                if (await repositorio.ExisteCorreo(dto.Correo!))
                    return BadRequest("Este correo electrónico ya está en uso");
                // Crear la billetera asociada
                var billetera = new Billeteras
                {
                    FechaCreacion = DateTime.Now,
                    Billera_Admin = false
                };
                await context.Billeteras.AddAsync(billetera);
                await context.SaveChangesAsync();
                 //  Crear el usuario
                var usuario = new Usuarios
                {
                    BilleteraId = billetera.Id,
                    CUIL = dto.CUIL,
                    Nombre = dto.Nombre!,
                    Apellido = dto.Apellido!,
                    Domicilio = dto.Domicilio!,
                    FechaNacimiento = dto.FechaNacimiento,
                    Correo = dto.Correo!,
                    Telefono = dto.Telefono!,
                    PasswordHash = dto.Password!,
                    EsAdmin = false
                };

                await repositorio.Insert(usuario);

                var cuenta = await CrearCuentaAutomaticamente(billetera.Id);
                var tipoCuentaCreados = await CrearTiposCuentaPorDefecto(cuenta.Id, billetera.Id, usuario);

                return Ok(new
                {
                    mensaje = "Usuario registrado exitosamente",
                    usuarioId = usuario.Id,
                    billetarId = billetera.Id,
                    cuentaId = cuenta.Id,
                    numCuenta = cuenta.NumCuenta,
                    tipoCuentaCreados = tipoCuentaCreados.Select(tc => new
                    {
                        id = tc.Id,
                        moneda = tc.Moneda_Tipo,
                        saldo = tc.Saldo
                    }).ToList()
                });  
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                // Error de duplicado a nivel de base de datos
                if (ex.InnerException.Message.Contains("CUIL"))
                    return BadRequest(new { mensaje = "Este CUIL ya se encuentra registrado en el sistema" });
                if (ex.InnerException.Message.Contains("Correo"))
                    return BadRequest(new { mensaje = "Este correo electrónico ya está en uso" });

                return BadRequest(new { mensaje = "Ya existe un registro con estos datos" });
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = $"Error al crear el registro: {e.InnerException?.Message ?? e.Message}" });
            }
        }


        private async Task<Cuenta> CrearCuentaAutomaticamente(int billeteraId)
        {
            var cuentasExistentes = await context.Cuentas.Where(c => c.BilleteraId == billeteraId)
                .CountAsync();

            int contador = cuentasExistentes + 1;

            var cuenta = new Cuenta
            {
                BilleteraId = billeteraId,
                NumCuenta = $"C{contador:D4}-B{billeteraId}"
            };

            await context.Cuentas.AddAsync(cuenta);
            await context.SaveChangesAsync();
            return cuenta;

        }

        private async Task<List<TipoCuenta>> CrearTiposCuentaPorDefecto(int cuentaId, int billeteraId, Usuarios usuario)
        {
            var tiposCuentaCreados = new List<TipoCuenta>();

            // Buscar las monedas habilitadas (ARS y USD por defecto)
            var monedasHabilitadas = await context.Monedas
                .Where(m => m.Habilitada && (m.CodISO == "ARS" || m.CodISO == "USD"))
                .ToListAsync();

            if (!monedasHabilitadas.Any())
            {
                Console.WriteLine("⚠️ No hay monedas habilitadas en la base de datos.");
                return tiposCuentaCreados; // Retornar lista vacía
            }

            // Crear una Caja de Ahorro por cada moneda habilitada
            foreach (var moneda in monedasHabilitadas)
            {
                //  GENERAR ALIAS (igual que en TipoCuentaController)
                var aliasBase = $"{usuario.Nombre}.{usuario.Apellido}.{moneda.CodISO}";

                // Limpiar caracteres especiales y espacios
                aliasBase = aliasBase
                    .Replace(" ", "")
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u")
                    .Replace("ñ", "n");

                // Verificar si ya existe y agregar número si es necesario
                var aliasExiste = await context.TiposCuentas.AnyAsync(tc => tc.Alias == aliasBase);
                var aliasDefinitivo = aliasBase;
                var contador = 2;

                while (aliasExiste)
                {
                    aliasDefinitivo = $"{aliasBase}{contador}";
                    aliasExiste = await context.TiposCuentas.AnyAsync(tc => tc.Alias == aliasDefinitivo);
                    contador++;
                }

                Console.WriteLine($"✅ Alias generado: {aliasDefinitivo}");

                // 🔥 CREAR TIPO CUENTA CON ALIAS
                var tipoCuenta = new TipoCuenta
                {
                    CuentaId = cuentaId,
                    MonedaId = moneda.Id,
                    TC_Nombre = "Caja de Ahorro",
                    Alias = aliasDefinitivo, 
                    Moneda_Tipo = moneda.CodISO,
                    Saldo = 0,
                    EsCuentaDemo = false
                };

                await context.TiposCuentas.AddAsync(tipoCuenta);
                tiposCuentaCreados.Add(tipoCuenta);
            }

            await context.SaveChangesAsync();
            return tiposCuentaCreados;
        }



        /// Inicia sesión de un usuario.

        [HttpPost("login")]
        public async Task<ActionResult> IniciarSesion(UsuariosDTO.Login dto)
        {
            try
            {
                var usuario = await repositorio.GetAll()
                    .Where(u => u.CUIL == dto.CUIL && u.EsAdmin == false)
                    .FirstOrDefaultAsync();

                if (usuario == null)
                    return Unauthorized("CUIL incorrecto");

                // Verificar contraseña
                if (usuario.PasswordHash != dto.Password) 
                    return Unauthorized("Contraseña incorrecta");

                // Mapear a DTO
                var usuarioDTO = new UsuariosDTO
                {
                    Id = usuario.Id,
                    BilleteraId = usuario.BilleteraId,
                    CUIL = usuario.CUIL,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Domicilio = usuario.Domicilio,
                    FechaNacimiento = usuario.FechaNacimiento,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    EsAdmin = usuario.EsAdmin
                };

                return Ok(new { mensaje = "Inicio de sesión exitoso", usuario = usuarioDTO });
            }
            catch (Exception e)
            {
                return BadRequest($"Error al iniciar sesión: {e.Message}");
            }
        }
        /// Inicia sesión de un administrador.
        [HttpPost("admin/login")]
        public async Task<ActionResult> LoginAdmin([FromBody] UsuariosDTO.LoginAdmin dto)
        {
            try
            {
                // Buscar el único admin
                var admin = await repositorio.GetAll()
                    .Where(u => u.EsAdmin == true)
                    .FirstOrDefaultAsync();

                if (admin == null)
                    return Unauthorized("No existe usuario administrador");

                // Desencriptar contraseña
                string passwordDesencriptada = _encryptionService.Desencriptar(admin.PasswordHash);

                if (passwordDesencriptada != dto.Password)
                    return Unauthorized("Contraseña de administrador incorrecta");

                return Ok(new
                {
                    mensaje = "Acceso de administrador concedido",
                    esAdmin = true
                });
            }
            catch (Exception e)
            {
                return BadRequest($"Error al iniciar sesión de admin: {e.Message}");
            }
        }


        /// Obtiene todos los usuarios.

        [HttpGet]
        public async Task<ActionResult<List<UsuariosDTO>>> GetAll()
        {
            try
            {
                var usuarios = await repositorio.GetAll()
                    .Select(u => new UsuariosDTO
                    {
                        Id = u.Id,
                        BilleteraId = u.BilleteraId,
                        CUIL = u.CUIL,
                        Nombre = u.Nombre,
                        Apellido = u.Apellido,
                        Domicilio = u.Domicilio,
                        FechaNacimiento = u.FechaNacimiento,
                        Correo = u.Correo,
                        Telefono = u.Telefono,
                        EsAdmin = u.EsAdmin
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception e)
            {
                return BadRequest($"Error al obtener usuarios: {e.Message}");
            }
        }

   
        /// Obtiene un usuario por su ID.
       
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuariosDTO>> GetById(int id)
        {
            try
            {
                var usuario = await repositorio.GetById(id);
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                var usuarioDTO = new UsuariosDTO
                {
                    Id = usuario.Id,
                    BilleteraId = usuario.BilleteraId,
                    CUIL = usuario.CUIL,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Domicilio = usuario.Domicilio,
                    FechaNacimiento = usuario.FechaNacimiento,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    EsAdmin = usuario.EsAdmin
                };

                return Ok(usuarioDTO);
            }
            catch (Exception e)
            {
                return BadRequest($"Error al obtener el usuario: {e.Message}");
            }
        }
        }
    

}

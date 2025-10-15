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

       
        /// Registra un nuevo usuario en el sistema.
       
        
        [HttpPost("registro")]
        public async Task<ActionResult> RegistrarUsuario([FromBody] UsuariosDTO.Registro dto)
        {
            try
            {
                //  Validar duplicados
                if (await repositorio.ExisteCUIL(dto.CUIL))
                    return BadRequest("El CUIL ya está registrado");

                if (await repositorio.ExisteCorreo(dto.Correo))
                    return BadRequest("El correo ya está registrado");
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
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Domicilio = dto.Domicilio,
                    FechaNacimiento = dto.FechaNacimiento,
                    Correo = dto.Correo,
                    Telefono = dto.Telefono,
                    PasswordHash = dto.Password,
                    EsAdmin = false
                };

                await repositorio.Insert(usuario);
                return Ok(new
                {
                    mensaje = "Usuario registrado exitosamente",
                    usuarioId = usuario.Id
                });  
            }
            catch (Exception e) 
            {
                return BadRequest($"Error al crear el registro: {e.InnerException?.Message ?? e.Message}");
            }
        }

       
        /// Inicia sesión de un usuario.
      
        [HttpPost("inicio-sesion")]
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

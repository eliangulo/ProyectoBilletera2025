using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using Billetera.Shared.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Billetera.Shared.DTOS;
using Billetera.Repositorio.Repositorio;

namespace Billetera.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosRepositorio<Usuarios> repositorio;
        private readonly AppDbContext context;

        public UsuariosController(AppDbContext context,
            IUsuariosRepositorio<Usuarios> repositorio)
        {
            this.repositorio = repositorio;
            this.context = context;
        }

        
        /// Registra un nuevo usuario en el sistema
       
        [HttpPost("registro")] ///api/controller/registro
        public async Task<ActionResult<int>> RegistrarUsuario(UsuariosDTO.Registro dto)
        {
            try
            {
                // Validar que el CUIL no exista
                if (await repositorio.ExisteCUIL(dto.CUIL))
                    return BadRequest("El CUIL ya está registrado");

                // Validar que el correo no exista
                if (await repositorio.ExisteCorreo(dto.Correo))
                    return BadRequest("El correo ya está registrado");

                // Crear la billetera
                var billetera = new Billeteras
                {
                    FechaCreacion = DateTime.Now,
                    Billera_Admin = dto.EsAdmin
                };
                await context.Billetera.AddAsync(billetera);
                await context.SaveChangesAsync();

                // Hashear la contraseña
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Crear el usuario
                Usuarios usuario = new Usuarios
                {
                    BilleteraId = billetera.Id,
                    CUIL = dto.CUIL,
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Domicilio = dto.Domicilio,
                    FechaNacimiento = dto.FechaNacimiento,
                    Correo = dto.Correo,
                    Telefono = dto.Telefono,
                    PasswordHash = passwordHash,
                    EsAdmin = dto.EsAdmin
                };

                await repositorio.Insert(usuario);
                return Ok(new { mensaje = "Usuario registrado exitosamente", usuarioId = usuario.Id });
            }
            catch (Exception e)
            {
                return BadRequest($"Error al crear el registro: {e.InnerException?.Message ?? e.Message}");
            }
        }

    
        /// Inicia sesión de un usuario
    
        [HttpPost("inicio-sesion")]
        public async Task<ActionResult<UsuariosDTO>> IniciarSesion(UsuariosDTO.Login dto)
        {
            try
            {
                var usuario = await repositorio.GetAll()
                    .Where(u => u.Correo == dto.Correo && u.CUIL == dto.CUIL)
                    .FirstOrDefaultAsync();

                if (usuario == null)
                    return Unauthorized("CUIL o correo incorrecto");

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
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

                return Ok(usuarioDTO);
            }
            catch (Exception e)
            {
                return BadRequest($"Error al iniciar sesión: {e.Message}");
            }
        }

    
        /// Obtiene todos los usuarios
    
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

      
        /// Obtiene un usuario por ID
        
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
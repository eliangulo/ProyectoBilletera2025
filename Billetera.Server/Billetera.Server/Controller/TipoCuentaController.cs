using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Shared.DTOS;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billetera.Server.Controller
{
    [ApiController]
    [Route("api/TipoCuenta")]
    public class TipoCuentaController : ControllerBase
    {
        private readonly ITipoCuentaRepositorio repositorio;
        private readonly AppDbContext context;
        public TipoCuentaController(ITipoCuentaRepositorio repositorio, AppDbContext context)
        {
            this.repositorio = repositorio;
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoCuentaIdDTO>>> GetTiposCuentas()
        {
            var entidades = await repositorio.Select();
            var dtos = entidades.Select(e => new TipoCuentaIdDTO
            {
                Id = e.Id,
                TC_Nombre = e.TC_Nombre,
                Alias = e.Alias,
                Moneda_Tipo = e.Moneda_Tipo,
                MonedaId = e.MonedaId,
                Saldo = e.Saldo,
                CuentaId = e.CuentaId,

            }).ToList();

            return Ok(dtos);
        }

        /// <summary>
        ///Ahora retorna TipoCuentaIdDTO con el Id real del TipoCuenta
        /// </summary>
        [HttpGet("billetera/{billeteraId:int}")]
        public async Task<ActionResult<List<TipoCuentaIdDTO>>> GetTiposCuentaPorBilletera(int billeteraId)
        {
            try
            {
                var tiposCuenta = await repositorio.GetTiposCuentaPorBilletera(billeteraId);

                // Ahora incluye el Id del TipoCuenta
                var dtos = tiposCuenta.Select(tc => new TipoCuentaIdDTO
                {
                    Id = tc.Id, // Este es el ID REAL del TipoCuenta (antes faltaba)
                    TC_Nombre = tc.TC_Nombre,
                    Alias = tc.Alias,
                    Moneda_Tipo = tc.Moneda_Tipo,
                    MonedaId = tc.MonedaId,
                    Saldo = tc.Saldo,
                    EsCuentaDemo = tc.EsCuentaDemo,
                    CuentaId = tc.CuentaId,
                    SaldoDisponible = tc.SaldoDisponible
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener tipos de cuenta: {ex.Message}");
            }
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<TipoCuentaIdDTO>> GetById(int Id)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
            {
                return NotFound($"Tipo de cuenta con Id {Id} no encontrado.");
            }

            var dto = new TipoCuentaIdDTO
            {
                Id = entidad.Id,
                TC_Nombre = entidad.TC_Nombre,
                Moneda_Tipo = entidad.Moneda_Tipo,
                MonedaId = entidad.MonedaId,
                CuentaId = entidad.CuentaId,
                Saldo = entidad.Saldo
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<TipoCuentaIdDTO>> Create(TipoCuentaDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos no válidos.");

            try
            {
                //Obtener información del usuario para generar el alias
                var cuenta = await context.Cuentas
                    .Include(c => c.Billetera)
                        .ThenInclude(b => b!.Usuarios)
                    .FirstOrDefaultAsync(c => c.Id == dto.CuentaId);

                if (cuenta == null)
                    return NotFound("Cuenta no encontrada.");

                var usuario = cuenta.Billetera?.Usuarios?.FirstOrDefault();
                if (usuario == null)
                    return NotFound("Usuario no encontrado.");

                // Generar alias único
                // Formato: Nombre.Apellido.Moneda
                // Ejemplo: Belen.Tejeda.USD
                var aliasBase = $"{usuario.Nombre}.{usuario.Apellido}.{dto.Moneda_Tipo}";

                // Limpiar caracteres especiales y espacios
                aliasBase = aliasBase
                    .Replace(" ", "")
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u")
                    .Replace("ñ", "n");

                //Verificar si ya existe y agregar número si es necesario
                var aliasExiste = await context.TiposCuentas.AnyAsync(tc => tc.Alias == aliasBase);
                var aliasDefinitivo = aliasBase;
                var contador = 2;

                while (aliasExiste)
                {
                    aliasDefinitivo = $"{aliasBase}{contador}";
                    aliasExiste = await context.TiposCuentas.AnyAsync(tc => tc.Alias == aliasDefinitivo);
                    contador++;
                }

                // Crear la entidad con el alias generado
                var entidad = new TipoCuenta
                {
                    TC_Nombre = dto.TC_Nombre,
                    Alias = aliasDefinitivo, // Asignar alias
                    Moneda_Tipo = dto.Moneda_Tipo,
                    MonedaId = dto.MonedaId,
                    Saldo = dto.Saldo,
                    CuentaId = dto.CuentaId,
                    EsCuentaDemo = dto.EsCuentaDemo
                };

                var id = await repositorio.Insert(entidad);

                var nuevoDTO = new TipoCuentaIdDTO
                {
                    Id = id,
                    TC_Nombre = dto.TC_Nombre,
                    Alias = aliasDefinitivo, //Incluir en respuesta
                    Moneda_Tipo = dto.Moneda_Tipo,
                    MonedaId = dto.MonedaId,
                    Saldo = dto.Saldo,
                    CuentaId = dto.CuentaId,
                    EsCuentaDemo = dto.EsCuentaDemo
                };

                return CreatedAtAction(nameof(GetById), new { Id = id }, nuevoDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear tipo de cuenta: {ex.Message}");
            }


        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Update(int Id, TipoCuentaDTO dto)
        {
            var entidad = await repositorio.SelectById(Id);
            if (entidad == null)
                return NotFound($"Tipo de cuenta con Id {Id} no encontrado.");

            entidad.TC_Nombre = dto.TC_Nombre;
            entidad.Moneda_Tipo = dto.Moneda_Tipo;
            entidad.MonedaId = dto.MonedaId;
            entidad.Saldo = dto.Saldo;
            entidad.CuentaId = dto.CuentaId;

            var actualizado = await repositorio.Update(Id, entidad);
            if (!actualizado)
                return BadRequest("No se pudo actualizar el tipo de cuenta.");

            return Ok("Tipo de cuenta actualizado correctamente.");
        }

        // Este método permite actualizar unicamente el alias de una cuenta específica
        [HttpPut("{Id:int}/actualizaralias")]
        public async Task<ActionResult> ActualizarAlias(int Id, [FromBody] string nuevoAlias)
        {
            try
            {
                // Validar que el alias no esté vacío
                if (string.IsNullOrWhiteSpace(nuevoAlias))
                    return BadRequest("El alias no puede estar vacío.");

                // Limpiar el alias, como por ejemplo quitar espacios, caracteres especiales, tildes
                var aliasLimpio = nuevoAlias
                    .Trim()
                    .Replace(" ", "")
                    .Replace("á", "a").Replace("Á", "A")
                    .Replace("é", "e").Replace("É", "E")
                    .Replace("í", "i").Replace("Í", "I")
                    .Replace("ó", "o").Replace("Ó", "O")
                    .Replace("ú", "u").Replace("Ú", "U")
                    .Replace("ñ", "n").Replace("Ñ", "N");

                // Validar que no contenga caracteres prohibidos
                if (aliasLimpio.Contains("/") || aliasLimpio.Contains("\\") ||
                    aliasLimpio.Contains("'") || aliasLimpio.Contains("\""))
                    return BadRequest("El alias no puede contener /, \\, ' o \"");

                // Verificar que el alias no esté ya en uso por otra cuenta
                var aliasExiste = await context.TiposCuentas
                    .AnyAsync(tc => tc.Alias == aliasLimpio && tc.Id != Id);

                if (aliasExiste)
                    return BadRequest($"El alias '{aliasLimpio}' ya está en uso.");

                // Buscar la cuenta
                var tipoCuenta = await repositorio.SelectById(Id);
                if (tipoCuenta == null)
                    return NotFound($"Cuenta con Id {Id} no encontrada.");

                // Actualizar solo el alias
                tipoCuenta.Alias = aliasLimpio;

                var actualizado = await repositorio.Update(Id, tipoCuenta);
                if (!actualizado)
                    return BadRequest("No se pudo actualizar el alias.");

                return Ok(new { mensaje = "Alias actualizado correctamente", nuevoAlias = aliasLimpio });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar alias: {ex.Message}");
            }
        }

        // Este método busca una cuenta usando su alias y devuelve datos que pueden ser publicos
        //Es decir no devuelve información sensible
        [HttpGet("buscarporalias/{alias}")]
        public async Task<ActionResult<TipoCuentaIdDTO>> BuscarPorAlias(string alias)
        {
            try
            {
                // Buscar la cuenta con ese alias
                var tipoCuenta = await context.TiposCuentas
                    .Include(tc => tc.Cuenta)
                        .ThenInclude(c => c!.Billetera)
                            .ThenInclude(b => b!.Usuarios)
                    .FirstOrDefaultAsync(tc => tc.Alias == alias);

                if (tipoCuenta == null)
                    return NotFound($"No se encontró ninguna cuenta con el alias '{alias}'");

                // Obtener el nombre del usuario dueño de la cuenta
                var usuario = tipoCuenta.Cuenta?.Billetera?.Usuarios?.FirstOrDefault();
                var nombreCompleto = usuario != null ? $"{usuario.Nombre} {usuario.Apellido}" : "Usuario desconocido";
                var numCuenta = tipoCuenta.Cuenta?.NumCuenta ?? "";

                // Crear el DTO con toda la información
                var resultado = new 
                {
                    NombreCompleto = nombreCompleto,
                    NumCuenta = numCuenta,
                    TC_Nombre = tipoCuenta.TC_Nombre,
                    Alias = tipoCuenta.Alias,
                    Moneda_Tipo = tipoCuenta.Moneda_Tipo
                };

                // Devolver info adicional del dueño
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por alias: {ex.Message}");
            }
        }

        //Este método busca una cuenta usando su alias y devuelve el ID necesario para hacer transferencias
        [HttpGet("obteneridporalias/{alias}")]
        public async Task<ActionResult> ObtenerIdPorAlias(string alias)
        {
            try
            {
                Console.WriteLine($"Buscando alias '{alias}'");

                var tipoCuenta = await context.TiposCuentas
                    .Include(tc => tc.Cuenta)
                        .ThenInclude(c => c!.Billetera)
                            .ThenInclude(b => b!.Usuarios)
                    .FirstOrDefaultAsync(tc => tc.Alias == alias);

                if (tipoCuenta == null)
                {
                    Console.WriteLine($"No se encontró alias '{alias}'");
                    return NotFound($"No se encontró ninguna cuenta con el alias '{alias}'");
                }

                Console.WriteLine($"Alias encontrado - ID: {tipoCuenta.Id}, Alias: {tipoCuenta.Alias}");

                var usuario = tipoCuenta.Cuenta?.Billetera?.Usuarios?.FirstOrDefault();

                var resultado = new
                {
                    Id = tipoCuenta.Id,
                    NombreCompleto = usuario != null ? $"{usuario.Nombre} {usuario.Apellido}" : "Usuario desconocido",
                    NumCuenta = tipoCuenta.Cuenta?.NumCuenta ?? "",
                    Alias = tipoCuenta.Alias,
                    TC_Nombre = tipoCuenta.TC_Nombre,
                    Moneda_Tipo = tipoCuenta.Moneda_Tipo
                };

                Console.WriteLine($"Devolviendo {System.Text.Json.JsonSerializer.Serialize(resultado)}");
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return StatusCode(500, $"Error al obtener ID por alias: {ex.Message}");
            }
        }


        [HttpPost("generar-alias-masivo")]
        public async Task<ActionResult> GenerarAliasMasivo()
        {
            try
            {
                // Obtener TODAS las TipoCuentas (no solo las sin alias)
                var todasLasCuentas = await context.TiposCuentas
                    .Include(tc => tc.Cuenta)
                        .ThenInclude(c => c!.Billetera)
                            .ThenInclude(b => b!.Usuarios)
                    .ToListAsync();

                if (!todasLasCuentas.Any())
                    return Ok("No hay cuentas en el sistema.");

                int contadorActualizadas = 0;
                var ejemplos = new List<object>();

                foreach (var tipoCuenta in todasLasCuentas)
                {
                    // Regenerar alias SIEMPRE (incluso si ya tiene uno)

                    var usuario = tipoCuenta.Cuenta?.Billetera?.Usuarios?.FirstOrDefault();

                    if (usuario == null)
                    {
                        // Si no hay usuario, asignar alias genérico
                        tipoCuenta.Alias = $"Cuenta{tipoCuenta.Id}";
                        contadorActualizadas++;
                        ejemplos.Add(new { id = tipoCuenta.Id, nombre = tipoCuenta.TC_Nombre, alias = tipoCuenta.Alias });
                        continue;
                    }

                    //Generar alias: Nombre.Apellido.Moneda
                    var aliasBase = $"{usuario.Nombre}.{usuario.Apellido}.{tipoCuenta.Moneda_Tipo}"
                        .Replace(" ", "")
                        .Replace("á", "a").Replace("Á", "A")
                        .Replace("é", "e").Replace("É", "E")
                        .Replace("í", "i").Replace("Í", "I")
                        .Replace("ó", "o").Replace("Ó", "O")
                        .Replace("ú", "u").Replace("Ú", "U")
                        .Replace("ñ", "n").Replace("Ñ", "N");

                    // Verificar si existe y agregar número
                    var aliasExiste = todasLasCuentas
                        .Any(tc => tc.Alias == aliasBase && tc.Id != tipoCuenta.Id);

                    var aliasDefinitivo = aliasBase;
                    var contador = 2;

                    while (aliasExiste)
                    {
                        aliasDefinitivo = $"{aliasBase}{contador}";
                        aliasExiste = todasLasCuentas
                            .Any(tc => tc.Alias == aliasDefinitivo && tc.Id != tipoCuenta.Id);
                        contador++;
                    }

                    tipoCuenta.Alias = aliasDefinitivo;
                    contadorActualizadas++;
                    ejemplos.Add(new { id = tipoCuenta.Id, nombre = tipoCuenta.TC_Nombre, alias = tipoCuenta.Alias });
                }

                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Alias generados exitosamente",
                    cuentasActualizadas = contadorActualizadas,
                    ejemplos = ejemplos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar alias: {ex.Message}");
            }
        }


        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await repositorio.Existe(Id);
            if (!existe)
                return NotFound($"Tipo de cuenta con Id {Id} no encontrado.");

            try
            {
                var eliminado = await repositorio.Delete(Id);
                if (!eliminado)
                    return BadRequest("No se pudo eliminar el tipo de cuenta.");

                return Ok("Tipo de cuenta eliminada correctamente.");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_Cuentas_TiposCuentas"))
                {
                    return BadRequest("No se puede eliminar este tipo de cuenta porque hay cuentas asociadas.");
                }

                return StatusCode(500, "Error interno al intentar eliminar el tipo de cuenta.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }
    }
}
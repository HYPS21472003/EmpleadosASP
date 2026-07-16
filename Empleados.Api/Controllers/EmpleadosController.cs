using Empleados.Api.DTOs;
using Empleados.Data.Entities;
using Empleados.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Empleados.Api.Controllers
{
   [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;
        private readonly IWebHostEnvironment _env;

        public EmpleadosController(IEmpleadoService empleadoService, IWebHostEnvironment env)
        {
            _empleadoService = empleadoService;
            _env = env;
        }

        // GET: api/empleados?page=1&pageSize=10&nombre=Juan&apellido=Perez
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDto<EmpleadoResumenDto>))]
        public async Task<ActionResult<PagedResultDto<EmpleadoResumenDto>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? nombre = null,
            [FromQuery] string? apellido = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var empleados = await _empleadoService.ObtenerTodosAsync(nombre, apellido, page, pageSize);
            var totalCount = await _empleadoService.ContarTotalAsync(nombre, apellido);

            var items = empleados.Select(e => new EmpleadoResumenDto
            {
                Id = e.Id,
                NombreCompleto = $"{e.Nombre} {e.Apellido}",
                Puesto = e.Puesto,
                DepartamentoNombre = e.Departamento?.Nombre ?? "Sin Asignar",
                Salario = e.Salario,
                FechaContratacion = e.FechaContratacion,
                Estado = e.Estado.ToString(),
                FotoRuta = e.FotoRuta
            });

            var result = new PagedResultDto<EmpleadoResumenDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/empleados/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Empleado))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Empleado>> GetPorId(int id)
        {
            try
            {
                var empleado = await _empleadoService.ObtenerPorIdAsync(id);

                if (empleado == null)
                    return NotFound(new { Mensaje = $"Empleado con ID {id} no fue encontrado en la base de datos." });

                return Ok(empleado);
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    ErrorInterno = ex.Message,
                    Detalle = ex.InnerException?.Message ?? "Sin detalle extra"
                });
            }
        }

        // POST: api/empleados
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Empleado))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Empleado>> Post([FromBody] EmpleadoUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nuevoEmpleado = new Empleado
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Puesto = dto.Puesto,
                Salario = dto.Salario,
                FechaNacimiento = dto.FechaNacimiento,
                FechaContratacion = dto.ContactacionFecha,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                CorreoElectronico = dto.CorreoElectronico,
                DepartamentoId = dto.DepartamentoId,
                FotoRuta = "uploads/empleados/default.png", // Foto por defecto
                Estado = EstadoEmpleado.Activo
            };

            var creado = await _empleadoService.CrearAsync(nuevoEmpleado);
            return CreatedAtAction(nameof(GetPorId), new { id = creado.Id }, creado);
        }

        // PUT: api/empleados/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] EmpleadoUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var empleadoAEditar = new Empleado
            {
                Id = id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Puesto = dto.Puesto,
                Salario = dto.Salario,
                FechaNacimiento = dto.FechaNacimiento,
                FechaContratacion = dto.ContactacionFecha,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                CorreoElectronico = dto.CorreoElectronico,
                DepartamentoId = dto.DepartamentoId
            };

            try
            {
                var resultado = await _empleadoService.ActualizarAsync(empleadoAEditar);
                if (!resultado) return NotFound(new { Mensaje = $"Empleado con ID {id} no existe." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) 
            {
                
                if (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    return BadRequest(new { Mensaje = $"El ID de Departamento ({dto.DepartamentoId}) especificado no existe en el sistema." });
                }

                // Cualquier otro error imprevisto de persistencia
                return StatusCode(StatusCodes.Status500InternalServerError, new { Mensaje = "Ocurrió un error inesperado al guardar los cambios en la base de datos." });
            }
        }

        // PATCH: api/empleados/{id}/desactivar
        [HttpPatch("{id}/desactivar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Desactivar(int id)
        {
            var resultado = await _empleadoService.DesactivarAsync(id);
            if (!resultado) return BadRequest(new { Mensaje = "No se pudo desactivar el empleado. Asegúrese de que existe y no esté Despedido." });

            return NoContent();
        }

        // PATCH: api/empleados/{id}/despedir
        [HttpPatch("{id}/despedir")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Despedir(int id)
        {
            var resultado = await _empleadoService.DespedirAsync(id);
            if (!resultado) return BadRequest(new { Mensaje = "No se pudo registrar el despido. Asegúrese de que el empleado existe y no haya sido despedido anteriormente." });

            return NoContent();
        }

        // POST: api/empleados/{id}/foto
        [HttpPost("{id}/foto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubirFoto(int id, IFormFile foto)
        {
            var empleado = await _empleadoService.ObtenerPorIdAsync(id);
            if (empleado == null) return NotFound(new { Mensaje = "Empleado no encontrado." });
            if (empleado.Estado == EstadoEmpleado.Despedido) return BadRequest(new { Mensaje = "No se puede cambiar la foto de un empleado despedido." });

            if (foto == null || foto.Length == 0) return BadRequest(new { Mensaje = "Archivo no válido." });

            // Validar extensión
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(foto.FileName).ToLower();
            if (!extensionesPermitidas.Contains(extension)) return BadRequest(new { Mensaje = "Formato de imagen inválido. Solo JPG, JPEG y PNG." });

            // Crear carpeta física si no existe en wwwroot de la API
            string carpetaUploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "empleados");
            if (!Directory.Exists(carpetaUploads))
            {
                Directory.CreateDirectory(carpetaUploads);
            }

           
            string nombreArchivo = $"empleado_{id}{extension}"; 
            string rutaFisicaCompleta = Path.Combine(carpetaUploads, nombreArchivo);

            // Guardar físicamente en el servidor
            using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            // Guardar la ruta relativa en la Base de Datos
            empleado.FotoRuta = $"uploads/empleados/{nombreArchivo}";
            await _empleadoService.ActualizarAsync(empleado);

            return Ok(new { Mensaje = "Fotografía subida exitosamente.", Ruta = empleado.FotoRuta });
        }
    }
}
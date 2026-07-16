using Empleados.Api.DTOs;
using Empleados.Data.Entities;
using Empleados.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Empleados.Api.Controllers
{
   [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentosController : ControllerBase
    {
        private readonly IDepartamentoService _departamentoService;

        public DepartamentosController(IDepartamentoService departamentoService)
        {
            _departamentoService = departamentoService;
        }

        // GET: api/departamentos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DepartamentoDto>))]
        public async Task<ActionResult<IEnumerable<DepartamentoDto>>> Get()
        {
            var departamentos = await _departamentoService.ObtenerTodosAsync();
            var dtos = departamentos.Select(d => new DepartamentoDto
            {
                Id = d.Id,
                Nombre = d.Nombre
            });

            return Ok(dtos);
        }

        // GET: api/departamentos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DepartamentoDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartamentoDto>> GetPorId(int id)
        {
            var departamento = await _departamentoService.ObtenerPorIdAsync(id);
            if (departamento == null) return NotFound(new { Mensaje = $"Departamento con ID {id} no fue encontrado." });

            var dto = new DepartamentoDto
            {
                Id = departamento.Id,
                Nombre = departamento.Nombre
            };

            return Ok(dto);
        }

        // POST: api/departamentos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DepartamentoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartamentoDto>> Post([FromBody] DepartamentoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nuevoDepto = new Departamento
            {
                Nombre = dto.Nombre
            };

            var creado = await _departamentoService.CrearAsync(nuevoDepto);
            dto.Id = creado.Id;

            return CreatedAtAction(nameof(GetPorId), new { id = dto.Id }, dto);
        }

        // PUT: api/departamentos/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] DepartamentoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest(new { Mensaje = "El ID del parámetro no coincide con el ID del cuerpo." });

            var deptoAEditar = new Departamento
            {
                Id = dto.Id,
                Nombre = dto.Nombre
            };

            var resultado = await _departamentoService.ActualizarAsync(deptoAEditar);
            if (!resultado) return NotFound(new { Mensaje = $"No se pudo actualizar. Departamento con ID {id} no existe." });

            return NoContent();
        }

        // DELETE: api/departamentos/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eliminado = await _departamentoService.EliminarAsync(id);
                if (!eliminado) return NotFound(new { Mensaje = $"Departamento con ID {id} no existe." });

                return NoContent();
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Empleados.Api.DTOs
{
    public class DepartamentoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}
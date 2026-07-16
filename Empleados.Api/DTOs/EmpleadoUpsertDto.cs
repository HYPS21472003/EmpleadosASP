using System;
using System.ComponentModel.DataAnnotations;

namespace Empleados.Api.DTOs
{
    public class EmpleadoUpsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(75, ErrorMessage = "El nombre no puede exceder los 75 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(75, ErrorMessage = "El apellido no puede exceder los 75 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El puesto de trabajo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El puesto no puede exceder los 100 caracteres.")]
        public string Puesto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El salario es obligatorio.")]
        [Range(0.01, 9999999.99, ErrorMessage = "El salario debe ser mayor a 0.")]
        public decimal Salario { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La fecha de contratación es obligatoria.")]
        public DateTime ContactacionFecha { get; set; } // Nota: mapeado a FechaContratacion

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres.")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El departamento asociado es obligatorio.")]
        public int DepartamentoId { get; set; }
    }
}
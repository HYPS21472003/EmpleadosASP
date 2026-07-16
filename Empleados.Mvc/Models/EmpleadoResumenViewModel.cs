using System;

namespace Empleados.Mvc.Models
{
    public class EmpleadoResumenViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public string DepartamentoNombre { get; set; } = string.Empty;
        public decimal Salario { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string FotoRuta { get; set; } = string.Empty;
    }
}
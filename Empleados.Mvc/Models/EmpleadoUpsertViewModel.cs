using System;

namespace Empleados.Mvc.Models
{
    public class EmpleadoUpsertViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public decimal Salario { get; set; }
        public DateTime FechaNacimiento { get; set; } = DateTime.Now.AddYears(-20);
        public DateTime ContactacionFecha { get; set; } = DateTime.Now;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int DepartamentoId { get; set; }
    }
}
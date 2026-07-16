using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleados.Data.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public decimal Salario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaContratacion { get; set; }
        public DateTime? FechaTerminacion { get; set; } // Nulo si está activo
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string FotoRuta { get; set; } = string.Empty; // Ruta relativa física
        public EstadoEmpleado Estado { get; set; } = EstadoEmpleado.Activo;

        // Relación con Departamento (Clave Foránea)
        public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleados.Data.Entities
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación: Un departamento tiene muchos empleados (Relación 1:N)
        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}

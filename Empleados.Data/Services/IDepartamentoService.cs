using System.Collections.Generic;
using System.Threading.Tasks;
using Empleados.Data.Entities;

namespace Empleados.Data.Services
{
    public interface IDepartamentoService
    {
        Task<IEnumerable<Departamento>> ObtenerTodosAsync();
        Task<Departamento?> ObtenerPorIdAsync(int id);
        Task<Departamento> CrearAsync(Departamento departamento);
        Task<bool> ActualizarAsync(Departamento departamento);
        Task<bool> EliminarAsync(int id); // Solo si no tiene empleados
    }
}

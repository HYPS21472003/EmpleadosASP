using System.Collections.Generic;
using System.Threading.Tasks;
using Empleados.Data.Entities;

namespace Empleados.Data.Services
{
    public interface IEmpleadoService
    {
        Task<IEnumerable<Empleado>> ObtenerTodosAsync(string? nombre, string? apellido, int page, int pageSize);
        Task<int> ContarTotalAsync(string? nombre, string? apellido);
        Task<Empleado?> ObtenerPorIdAsync(int id);
        Task<Empleado> CrearAsync(Empleado empleado);
        Task<bool> ActualizarAsync(Empleado empleado);
        Task<bool> DesactivarAsync(int id);
        Task<bool> DespedirAsync(int id);
    }
}

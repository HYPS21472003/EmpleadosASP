using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Empleados.Data.Entities;

namespace Empleados.Data.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly EmpleadosDbContext _context;

        public EmpleadoService(EmpleadosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Empleado>> ObtenerTodosAsync(string? nombre, string? apellido, int page, int pageSize)
        {
            var query = _context.Empleados.Include(e => e.Departamento).AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(e => e.Nombre.Contains(nombre));

            if (!string.IsNullOrWhiteSpace(apellido))
                query = query.Where(e => e.Apellido.Contains(apellido));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> ContarTotalAsync(string? nombre, string? apellido)
        {
            var query = _context.Empleados.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(e => e.Nombre.Contains(nombre));

            if (!string.IsNullOrWhiteSpace(apellido))
                query = query.Where(e => e.Apellido.Contains(apellido));

            return await query.CountAsync();
        }

        public async Task<Empleado?> ObtenerPorIdAsync(int id)
        {
            return await _context.Empleados
                .Include(e => e.Departamento)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Empleado> CrearAsync(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
            return empleado;
        }

        public async Task<bool> ActualizarAsync(Empleado empleado)
        {
            var existente = await _context.Empleados.FindAsync(empleado.Id);
            if (existente == null) return false;

            // Regla de negocio: Un empleado despedido no puede ser modificado
            if (existente.Estado == EstadoEmpleado.Despedido)
                throw new InvalidOperationException("No se permiten modificaciones en un empleado despedido.");

            // Actualizar campos permitidos
            _context.Entry(existente).CurrentValues.SetValues(empleado);

            // Forzar que no alteren el estado de despido por una edición común
            _context.Entry(existente).Property(e => e.Estado).IsModified = false;
            _context.Entry(existente).Property(e => e.FechaTerminacion).IsModified = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesactivarAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null || empleado.Estado == EstadoEmpleado.Despedido) return false;

            // Soft delete reversible (Alterna entre Activo e Inactivo)
            empleado.Estado = empleado.Estado == EstadoEmpleado.Activo
                ? EstadoEmpleado.Inactivo
                : EstadoEmpleado.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DespedirAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null || empleado.Estado == EstadoEmpleado.Despedido) return false;

            // Regla de negocio: Despedir es irreversible y setea la fecha de terminación
            empleado.Estado = EstadoEmpleado.Despedido;
            empleado.FechaTerminacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
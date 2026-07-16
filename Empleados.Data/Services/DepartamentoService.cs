using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Empleados.Data.Entities;

namespace Empleados.Data.Services
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly EmpleadosDbContext _context;

        public DepartamentoService(EmpleadosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Departamento>> ObtenerTodosAsync()
        {
            return await _context.Departamentos.ToListAsync();
        }

        public async Task<Departamento?> ObtenerPorIdAsync(int id)
        {
            return await _context.Departamentos.FindAsync(id);
        }

        public async Task<Departamento> CrearAsync(Departamento departamento)
        {
            _context.Departamentos.Add(departamento);
            await _context.SaveChangesAsync();
            return departamento;
        }

        public async Task<bool> ActualizarAsync(Departamento departamento)
        {
            _context.Entry(departamento).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var depto = await _context.Departamentos
                .Include(d => d.Empleados)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (depto == null) return false;

            // Regla de negocio: No se permite eliminar un departamento con empleados asociados
            if (depto.Empleados.Count > 0)
                throw new InvalidOperationException("No se puede eliminar un departamento que tiene empleados asociados.");

            _context.Departamentos.Remove(depto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
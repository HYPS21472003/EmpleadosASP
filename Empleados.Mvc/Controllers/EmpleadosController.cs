using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Empleados.Data;
using Empleados.Mvc.Models;

namespace Empleados.Mvc.Controllers
{
    [Authorize] // Autenticación obligatoria general por Cookies para entrar al controlador
    public class EmpleadosController : Controller
    {
        private readonly EmpleadosDbContext _context;

        public EmpleadosController(EmpleadosDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: Empleados/Index (Accesible para Admin y RRHH)
        // ==========================================
        public async Task<IActionResult> Index(int page = 1, string? nombre = null, string? apellido = null)
        {
            int pageSize = 5;
            ViewBag.NombreActual = nombre;
            ViewBag.ApellidoActual = apellido;

            var query = _context.Empleados
                .Include(e => e.Departamento)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(e => e.Nombre.Contains(nombre));

            if (!string.IsNullOrEmpty(apellido))
                query = query.Where(e => e.Apellido.Contains(apellido));

            int totalCount = await query.CountAsync();

            var empleados = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmpleadoResumenViewModel
                {
                    Id = e.Id,
                    NombreCompleto = e.Nombre + " " + e.Apellido,
                    Puesto = e.Puesto,
                    DepartamentoNombre = e.Departamento != null ? e.Departamento.Nombre : "",
                    Salario = e.Salario,
                    FechaContratacion = e.FechaContratacion,
                    Estado = (int)e.Estado == 1 ? "Activo" : ((int)e.Estado == 2 ? "Inactivo" : "Despedido"),
                    FotoRuta = e.FotoRuta ?? "uploads/empleados/default.png"
                })
                .ToListAsync();

            var resultado = new PagedResultViewModel<EmpleadoResumenViewModel>
            {
                Items = empleados,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return View(resultado);
        }

        // ==========================================
        // GET: Empleados/Create (Permitido para Admin y RRHH)
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new EmpleadoUpsertViewModel());
        }

        // ==========================================
        // POST: Empleados/Create (Permitido para Admin y RRHH)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoUpsertViewModel model, IFormFile? FotoArchivo)
        {
            if (!ModelState.IsValid) return View(model);

            var nuevoEmpleado = new Empleados.Data.Entities.Empleado
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Puesto = model.Puesto,
                Salario = model.Salario,
                Direccion = model.Direccion,
                Telefono = model.Telefono,
                CorreoElectronico = model.CorreoElectronico,
                DepartamentoId = model.DepartamentoId,
                FechaContratacion = DateTime.Now,
                Estado = (Empleados.Data.Entities.EstadoEmpleado)1
            };

            _context.Empleados.Add(nuevoEmpleado);
            await _context.SaveChangesAsync();

            if (FotoArchivo != null && FotoArchivo.Length > 0)
            {
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(wwwRootPath, "uploads", "empleados");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(FotoArchivo.FileName).ToLower();
                var nombreUnico = $"empleado_{nuevoEmpleado.Id}{extension}";
                var filePath = Path.Combine(uploadsFolder, nombreUnico);

                using (var fileStreamLocal = new FileStream(filePath, FileMode.Create))
                {
                    await FotoArchivo.CopyToAsync(fileStreamLocal);
                }

                nuevoEmpleado.FotoRuta = $"uploads/empleados/{nombreUnico}";
                _context.Empleados.Update(nuevoEmpleado);
                await _context.SaveChangesAsync();
            }

            TempData["Exito"] = "Empleado registrado exitosamente en la base de datos.";
            return RedirectToAction("Index");
        }

        // ==========================================
        // GET: Empleados/Edit/5 (Permitido para Admin y RRHH)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                TempData["Error"] = "El expediente de empleado solicitado no existe.";
                return RedirectToAction("Index");
            }

            if ((int)empleado.Estado == 3)
            {
                TempData["Error"] = "El expediente pertenece a un empleado despedido. Su estado es inmutable, solo lectura.";
                return RedirectToAction("Index");
            }

            var model = new EmpleadoUpsertViewModel
            {
                Id = empleado.Id,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                Puesto = empleado.Puesto,
                Salario = empleado.Salario,
                Direccion = empleado.Direccion,
                Telefono = empleado.Telefono,
                CorreoElectronico = empleado.CorreoElectronico,
                DepartamentoId = empleado.DepartamentoId
            };

            return View(model);
        }

        // ==========================================
        // POST: Empleados/Edit/5 (Permitido para Admin y RRHH)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmpleadoUpsertViewModel model, IFormFile? FotoArchivo)
        {
            if (!ModelState.IsValid) return View(model);

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            if ((int)empleado.Estado == 3)
            {
                TempData["Error"] = "Operación denegada. Un expediente despedido no admite modificaciones.";
                return RedirectToAction("Index");
            }

            empleado.Nombre = model.Nombre;
            empleado.Apellido = model.Apellido;
            empleado.Puesto = model.Puesto;
            empleado.Salario = model.Salario;
            empleado.Direccion = model.Direccion;
            empleado.Telefono = model.Telefono;
            empleado.CorreoElectronico = model.CorreoElectronico;
            empleado.DepartamentoId = model.DepartamentoId;

            if (FotoArchivo != null && FotoArchivo.Length > 0)
            {
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(wwwRootPath, "uploads", "empleados");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(FotoArchivo.FileName).ToLower();
                var nombreUnico = $"empleado_{id}{extension}";
                var filePath = Path.Combine(uploadsFolder, nombreUnico);

                using (var fileStreamLocal = new FileStream(filePath, FileMode.Create))
                {
                    await FotoArchivo.CopyToAsync(fileStreamLocal);
                }

                empleado.FotoRuta = $"uploads/empleados/{nombreUnico}";
            }

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Expediente y fotografía actualizados correctamente de manera autónoma.";
            return RedirectToAction("Index");
        }

        // ==========================================
        // ENDPOINT: Empleados/Desactivar/5 (Exclusivo Admin)
        // ==========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            empleado.Estado = (Empleados.Data.Entities.EstadoEmpleado)2;

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"El empleado {empleado.Nombre} ha sido cambiado a Inactivo con éxito.";
            return RedirectToAction("Index");
        }

        // ==========================================
        // ENDPOINT: Empleados/Reactivar/5 (Exclusivo Admin)
        // ==========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            empleado.Estado = (Empleados.Data.Entities.EstadoEmpleado)1;

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"El empleado {empleado.Nombre} ha sido reactivado al servicio operativo.";
            return RedirectToAction("Index");
        }

        // ==========================================
        // ENDPOINT: Empleados/Despedir/5 (Exclusivo Admin)
        // ==========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Despedir(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            empleado.Estado = (Empleados.Data.Entities.EstadoEmpleado)3;
            empleado.FechaTerminacion = DateTime.Now;

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Se ha procesado el despido lógico del empleado {empleado.Nombre}. Expediente bloqueado para edición.";
            return RedirectToAction("Index");
        }

        // ==========================================
        // GET: Empleados/Details (Accesible para Admin y RRHH)
        // ==========================================
        public async Task<IActionResult> Details(int id)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Departamento)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empleado == null) return NotFound();

            return View(empleado);
        }
    }
}
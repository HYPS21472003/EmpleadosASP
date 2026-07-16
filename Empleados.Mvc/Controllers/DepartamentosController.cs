using Microsoft.AspNetCore.Mvc;
using Empleados.Data.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Empleados.Mvc.Controllers
{
    [Authorize] // Protege todo el controlador con el sistema de Cookies global
    public class DepartamentosController : Controller
    {
        private readonly IDepartamentoService _departamentoService;

        public DepartamentosController(IDepartamentoService departamentoService)
        {
            _departamentoService = departamentoService;
        }

        // ==========================================
        // GET: Departamentos (Accesible para Admin y RRHH)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var departamentos = await _departamentoService.ObtenerTodosAsync();
            return View(departamentos);
        }

        // ==========================================
        // POST: Departamentos/Delete/5 (Exclusivo Admin)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // 🔐 REQUISITO PLIEGO: Bloqueo del lado del servidor para el rol RRHH
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eliminado = await _departamentoService.EliminarAsync(id);

                if (eliminado)
                {
                    TempData["Exito"] = "Departamento eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "El departamento especificado no existe.";
                }
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message; // No se puede eliminar un departamento con empleados asociados
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
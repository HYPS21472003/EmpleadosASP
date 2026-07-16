using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Empleados.Mvc.Services;

namespace Empleados.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Home/Index (Listado Principal)
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5, // Mostraremos 5 por página para notar la paginación fácilmente
            [FromQuery] string? nombre = null,
            [FromQuery] string? apellido = null)
        {
            // Regla de Seguridad: Si no hay Token JWT en la sesión, redirigir al Login
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Consultar datos desde la API usando el servicio
            var resultado = await _apiService.ObtenerEmpleadosAsync(page, pageSize, nombre, apellido);

            // Pasar los filtros actuales a la vista para mantener el texto escrito en las cajas de búsqueda
            ViewBag.NombreActual = nombre;
            ViewBag.ApellidoActual = apellido;

            return View(resultado);
        }

        public IActionResult Privacy()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
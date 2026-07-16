using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Empleados.Mvc.Models;

namespace Empleados.Mvc.Controllers
{
    public class AccountController : Controller
    {
        // Ya no necesitamos inyectar el DbContext para buscar usuarios porque los validaremos localmente
        public AccountController()
        {
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string? rolAsignado = null;
            string? usuarioNombre = null;

            // 1. Definimos credenciales fijas para los roles exigidos por el pliego
            if (model.Usuario == "admin@sistema.com" && model.Password == "Admin123")
            {
                usuarioNombre = "Administrador del Sistema";
                rolAsignado = "Admin"; // Rol con permisos totales
            }
            else if (model.Usuario == "rrhh@sistema.com" && model.Password == "Rrhh123")
            {
                usuarioNombre = "Operador de Recursos Humanos";
                rolAsignado = "RRHH"; // Rol limitado (solo gestiona empleados)
            }

            // 2. Si el usuario coincide con alguna de nuestras credenciales válidas
            if (rolAsignado != null && usuarioNombre != null)
            {
                // 3. Crear los Claims de Identidad con su Rol correspondiente
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioNombre),
                    new Claim(ClaimTypes.Email, model.Usuario),
                    new Claim(ClaimTypes.Role, rolAsignado) // Esto valida el [Authorize(Roles = "...")]
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. Firmar y guardar la Cookie encriptada en el navegador
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Empleados");
            }

            // Si las credenciales no coinciden
            ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
            return View(model);
        }

        // GET: Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Matar la sesión de cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Empleados.Mvc.Models;

namespace Empleados.Mvc.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7158/api/";
        }

        // Método auxiliar para añadir el Token JWT guardado en la sesión a cada petición
        private void AgregarTokenAutorizacion()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Realizar un Login y guardar el Token en la Sesión
        public async Task<bool> LoginAsync(string usuario, string password)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}Auth/login", new { usuario, password });
            if (!response.IsSuccessStatusCode) return false;

            var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (resultado != null && !string.IsNullOrEmpty(resultado.Token))
            {
                // Guardar el token en la sesión del usuario actual
                _httpContextAccessor.HttpContext?.Session.SetString("JWToken", resultado.Token);
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioLogueado", usuario);
                return true;
            }

            return false;
        }

        // Cerrar sesión limpiando la caché local
        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("JWToken");
            _httpContextAccessor.HttpContext?.Session.Remove("UsuarioLogueado");
        }

        // Obtener la lista paginada y filtrada de empleados de la API
        public async Task<PagedResultViewModel<EmpleadoResumenViewModel>?> ObtenerEmpleadosAsync(int page, int pageSize, string? nombre, string? apellido)
        {
            AgregarTokenAutorizacion();
            string query = $"Empleados?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(nombre)) query += $"&nombre={Uri.EscapeDataString(nombre)}";
            if (!string.IsNullOrEmpty(apellido)) query += $"&apellido={Uri.EscapeDataString(apellido)}";

            try
            {
                return await _httpClient.GetFromJsonAsync<PagedResultViewModel<EmpleadoResumenViewModel>>($"{_baseUrl}{query}");
            }
            catch
            {
                return null;
            }
        }

        // Obtener todos los departamentos para los selects de las vistas
        public async Task<List<DepartamentoViewModel>> ObtenerDepartamentosAsync()
        {
            AgregarTokenAutorizacion();
            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<List<DepartamentoViewModel>>($"{_baseUrl}Departamentos");
                return resultado ?? new List<DepartamentoViewModel>();
            }
            catch
            {
                return new List<DepartamentoViewModel>();
            }
        }
    }

    // Estructura interna para recibir la respuesta del Login de la API
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Empleados.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simulación de usuario administrativo para fines académicos/prueba
            if (request.Usuario == "admin" && request.Password == "Admin123*")
            {
                var secretKey = _config["Jwt:Key"] ?? "SuperSecretKeySistemasEmpleados2026_JWT_Token";
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Usuario),
                    new Claim(ClaimTypes.Role, "Administrador"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(2), // Expira en 2 horas
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var nuevoToken = tokenHandler.CreateToken(token);

                return Ok(new
                {
                    Token = tokenHandler.WriteToken(nuevoToken),
                    Expiracion = token.Expires
                });
            }

            return Unauthorized(new { Mensaje = "Usuario o contraseña incorrectos." });
        }
    }

    public class LoginRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
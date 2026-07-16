using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Empleados.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar soporte para controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// 2. Registrar el DbContext compartiendo la base de datos de SQL Server
builder.Services.AddDbContext<EmpleadosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// [CORRECCIÓN]: Registrar el servicio de Sesión requerido para leer tokens en controladores
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Registrar el esquema de Autenticación por Cookies (Requisito del Pliego)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Expira en 1 hora
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// 4. Registrar dependencias adicionales del sistema de datos
builder.Services.AddScoped<Empleados.Data.Services.IDepartamentoService, Empleados.Data.Services.DepartamentoService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// El orden de estos dos middlewares de seguridad es estricto e indispensable
app.UseAuthentication();
app.UseAuthorization();

// Ruta de arranque por defecto apuntando al controlador de Empleados
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Empleados}/{action=Index}/{id?}");

app.Run();
Sistema de Gestión de Empleados - EmpleadosASP
Este sistema modular consta de aplicaciones independientes que comparten una misma base de datos en SQL Server mediante una arquitectura de Persistencia Compartida (Shared Database). El desarrollo está estructurado bajo la Opción B (Solución Multicapa) para garantizar una separación estricta de responsabilidades.

1. Arquitectura de la Solución (Separación de Responsabilidades)
El código está estrictamente organizado en capas independientes para aislar la lógica de negocio de la presentación:

Empleados.Data: Biblioteca de clases que contiene las entidades de base de datos (Empleado, Departamento), el contexto de Entity Framework Core (EmpleadosDbContext) y los servicios de lógica de negocio.

Empleados.Api: Servicio REST público protegido con JWT Bearer Tokens. Gestiona la comunicación exclusivamente mediante DTOs (EmpleadoResumenDto, EmpleadoUpsertDto), aislando el modelo de datos de la red.

Empleados.Mvc: Portal web interno para administración de personal mediante controladores tradicionales y autenticación basada en Cookies. Utiliza ViewModels (LoginViewModel, EmpleadoUpsertViewModel) dedicados a la presentación.

Empleados.Tests: Proyecto de pruebas unitarias implementado en xUnit para verificar las reglas de negocio críticas del sistema.

2. Calidad, Validación y Manejo de Errores
Validación en Dos Capas:

Cliente: Implementada en las vistas de Empleados.Mvc mediante _ValidationScriptsPartial.cshtml, habilitando validaciones instantáneas con jQuery Validation.

Servidor: Validación robusta en los controladores de la API y el MVC mediante atributos de datos (Data Annotations) y la verificación estricta de ModelState.IsValid.

Manejo de Errores Seguro: El sistema tiene configurados middlewares globales de excepciones (app.UseExceptionHandler). Ningún error técnico interno, excepción de SQL Server o Stack Trace es expuesto al usuario final, redirigiendo a páginas de error amigables y registrando logs para diagnóstico técnico.

Seguridad de Configuración: Las cadenas de conexión, credenciales de desarrollo y secretos de firmado JWT residen únicamente en los archivos locales appsettings.Development.json y están estrictamente excluidos del control de versiones mediante el archivo .gitignore.

3. Instrucciones de Configuración Inicial y Clonación
Paso A: Clonar el Repositorio
git clone https://github.com/HYPS21472003/EmpleadosASP.git
cd EmpleadosASP

Paso B: Restaurar Paquetes NuGet
dotnet restore

Paso C: Configurar la Cadena de Conexión
Asegúrese de configurar la cadena de conexión correspondiente a su entorno de SQL Server en el archivo appsettings.Development.json tanto de Empleados.Mvc como de Empleados.Api:

"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=SistemaEmpleadosDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}

Paso D: Ejecutar las Migraciones
Abra la Consola del Administrador de Paquetes en Visual Studio, configure Empleados.Data como proyecto predeterminado y ejecute el comando para evitar conflictos de inicialización:

Update-Database -StartUpProject Empleados.Api -Project Empleados.Data

4. Cómo Levantar los Proyectos por Separado
El pliego exige que ambas aplicaciones puedan operar de manera autónoma. Utilice los siguientes comandos desde la raíz de la solución:

Para iniciar la Web API de forma independiente:
cd Empleados.Api
dotnet run

Para iniciar la Aplicación Web MVC de forma independiente:
cd Empleados.Mvc
dotnet run

5. Credenciales de Prueba Configuradas (Seed Data)
Utilice los siguientes usuarios preconfigurados en la base de datos para probar la seguridad basada en roles del sistema:

👤 Rol Administrador (Acceso Total): admin@sistema.com | Contraseña: Admin123

👤 Rol Recursos Humanos (Acceso Restringido): rrhh@sistema.com | Contraseña: Rrhh123
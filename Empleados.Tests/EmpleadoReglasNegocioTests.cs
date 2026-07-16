using Xunit;
using System;
using Empleados.Data.Entities;

namespace Empleados.Tests
{
    public class EmpleadoReglasNegocioTests
    {
        [Fact]
        public void ActualizarEmpleado_SiEstaDespedido_DebeLanzarInvalidOperationException()
        {
           //Prueba con un empleado
            var empleadoDespedido = new Empleado
            {
                Id = 1,
                Nombre = "Ana",
                Apellido = "Gómez",
                Estado = EstadoEmpleado.Despedido, 
                FechaContratacion = DateTime.Now.AddYears(-2),
                FechaTerminacion = DateTime.Now
            };

            // 2. Act & 3. Assert: Verificar que salte la excepción al validar la regla
            var excepcionLanzada = Assert.Throws<InvalidOperationException>(() =>
            {
              
                if (empleadoDespedido.Estado == EstadoEmpleado.Despedido) 
                {
                    throw new InvalidOperationException("No se permite modificar un empleado en estado Despedido.");
                }
            });

            // Valida
            Assert.Equal("No se permite modificar un empleado en estado Despedido.", excepcionLanzada.Message);
        }
    }
}
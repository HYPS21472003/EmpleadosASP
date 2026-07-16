using Microsoft.EntityFrameworkCore;
using Empleados.Data.Entities;
using System;

namespace Empleados.Data
{
    public class EmpleadosDbContext : DbContext
    {
        public EmpleadosDbContext(DbContextOptions<EmpleadosDbContext> options) : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; } = null!;
        public DbSet<Departamento> Departamentos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            // 1. CONFIGURACIÓN DE DEPARTAMENTO
           
            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("Departamentos");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Nombre)
                      .IsRequired()
                      .HasMaxLength(100);
            });

          
            // 2. CONFIGURACIÓN DE EMPLEADO
         
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleados");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(75);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(75);
                entity.Property(e => e.Puesto).IsRequired().HasMaxLength(100);

                // Precisión para salario (18 enteros, 2 decimales)
                entity.Property(e => e.Salario)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.FechaContratacion).IsRequired();
                entity.Property(e => e.FechaTerminacion).IsRequired(false);
                entity.Property(e => e.Direccion).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);

                entity.Property(e => e.CorreoElectronico).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.CorreoElectronico).IsUnique(); // Correo único

                entity.Property(e => e.FotoRuta).HasMaxLength(500);

                // Mapeo del Enumerador como Entero en BD
                entity.Property(e => e.Estado)
                      .HasConversion<int>()
                      .IsRequired();

                // Relación 1:N (Un departamento, muchos empleados)
                entity.HasOne(e => e.Departamento)
                      .WithMany(d => d.Empleados)
                      .HasForeignKey(e => e.DepartamentoId)
                      .OnDelete(DeleteBehavior.Restrict); // Regla: No borrar depto con empleados activos
            });

          
            // 3. SEED DATA (DATOS SEMILLA)
          

            // Departamentos
            modelBuilder.Entity<Departamento>().HasData(
                new Departamento { Id = 1, Nombre = "Recursos Humanos" },
                new Departamento { Id = 2, Nombre = "Tecnología de la Información" },
                new Departamento { Id = 3, Nombre = "Operaciones y Logística" }
            );

            // 10 Empleados Iniciales de Prueba
            modelBuilder.Entity<Empleado>().HasData(
                new Empleado { Id = 1, Nombre = "Carlos", Apellido = "Mendoza", Puesto = "Gerente de TI", Salario = 85000.00m, FechaNacimiento = new DateTime(1988, 5, 14), FechaContratacion = new DateTime(2020, 1, 15), Direccion = "Calle Principal 123", Telefono = "809-555-0101", CorreoElectronico = "c.mendoza@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 2 },
                new Empleado { Id = 2, Nombre = "Ana", Apellido = "Gómez", Puesto = "Generalista de RRHH", Salario = 45000.00m, FechaNacimiento = new DateTime(1992, 11, 23), FechaContratacion = new DateTime(2021, 3, 1), Direccion = "Av. Central 456", Telefono = "809-555-0102", CorreoElectronico = "a.gomez@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 1 },
                new Empleado { Id = 3, Nombre = "Luis", Apellido = "Rosario", Puesto = "Coordinador de Logística", Salario = 55000.00m, FechaNacimiento = new DateTime(1990, 8, 5), FechaContratacion = new DateTime(2019, 6, 10), Direccion = "Calle 4, No. 12", Telefono = "809-555-0103", CorreoElectronico = "l.rosario@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 3 },
                new Empleado { Id = 4, Nombre = "María", Apellido = "Peralta", Puesto = "Desarrolladora Fullstack", Salario = 70000.00m, FechaNacimiento = new DateTime(1995, 3, 30), FechaContratacion = new DateTime(2022, 2, 15), Direccion = "Residencial Altagracia B4", Telefono = "809-555-0104", CorreoElectronico = "m.peralta@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 2 },
                new Empleado { Id = 5, Nombre = "José", Apellido = "Martínez", Puesto = "Soporte Técnico", Salario = 35000.00m, FechaNacimiento = new DateTime(1997, 12, 12), FechaContratacion = new DateTime(2023, 5, 1), Direccion = "Calle Duarte 78", Telefono = "809-555-0105", CorreoElectronico = "j.martinez@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 2 },
                new Empleado { Id = 6, Nombre = "Elena", Apellido = "Castro", Puesto = "Directora de RRHH", Salario = 95000.00m, FechaNacimiento = new DateTime(1983, 4, 18), FechaContratacion = new DateTime(2015, 8, 22), Direccion = "Av. Anacaona E12", Telefono = "809-555-0106", CorreoElectronico = "e.castro@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 1 },
                new Empleado { Id = 7, Nombre = "Ricardo", Apellido = "Santos", Puesto = "Supervisor de Despacho", Salario = 48000.00m, FechaNacimiento = new DateTime(1991, 9, 25), FechaContratacion = new DateTime(2020, 11, 1), Direccion = "Calle Las Carreras 203", Telefono = "809-555-0107", CorreoElectronico = "r.santos@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 3 },
                new Empleado { Id = 8, Nombre = "Laura", Apellido = "Espinal", Puesto = "Analista de Reclutamiento", Salario = 40000.00m, FechaNacimiento = new DateTime(1994, 2, 14), FechaContratacion = new DateTime(2022, 9, 16), Direccion = "Urb. Real, Calle C", Telefono = "809-555-0108", CorreoElectronico = "l.espinal@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 1 },
                new Empleado { Id = 9, Nombre = "Pedro", Apellido = "Jiménez", Puesto = "Administrador de Sistemas", Salario = 65000.00m, FechaNacimiento = new DateTime(1989, 7, 7), FechaContratacion = new DateTime(2018, 4, 1), Direccion = "Ensanche Luperón No. 5", Telefono = "809-555-0109", CorreoElectronico = "p.jimenez@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 2 },
                new Empleado { Id = 10, Nombre = "Sofía", Apellido = "Vargas", Puesto = "Auxiliar de Almacén", Salario = 30000.00m, FechaNacimiento = new DateTime(1999, 10, 5), FechaContratacion = new DateTime(2023, 10, 1), Direccion = "Villa Consuelo, Calle 3", Telefono = "809-555-0110", CorreoElectronico = "s.vargas@organizacion.com", FotoRuta = "uploads/empleados/default.png", Estado = EstadoEmpleado.Activo, DepartamentoId = 3 }
            );
        }
    }
}
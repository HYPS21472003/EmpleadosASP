using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Empleados.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaContratacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaTerminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FotoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empleados_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departamentos",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Recursos Humanos" },
                    { 2, "Tecnología de la Información" },
                    { 3, "Operaciones y Logística" }
                });

            migrationBuilder.InsertData(
                table: "Empleados",
                columns: new[] { "Id", "Apellido", "CorreoElectronico", "DepartamentoId", "Direccion", "Estado", "FechaContratacion", "FechaNacimiento", "FechaTerminacion", "FotoRuta", "Nombre", "Puesto", "Salario", "Telefono" },
                values: new object[,]
                {
                    { 1, "Mendoza", "c.mendoza@organizacion.com", 2, "Calle Principal 123", 1, new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Carlos", "Gerente de TI", 85000.00m, "809-555-0101" },
                    { 2, "Gómez", "a.gomez@organizacion.com", 1, "Av. Central 456", 1, new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Ana", "Generalista de RRHH", 45000.00m, "809-555-0102" },
                    { 3, "Rosario", "l.rosario@organizacion.com", 3, "Calle 4, No. 12", 1, new DateTime(2019, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Luis", "Coordinador de Logística", 55000.00m, "809-555-0103" },
                    { 4, "Peralta", "m.peralta@organizacion.com", 2, "Residencial Altagracia B4", 1, new DateTime(2022, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "María", "Desarrolladora Fullstack", 70000.00m, "809-555-0104" },
                    { 5, "Martínez", "j.martinez@organizacion.com", 2, "Calle Duarte 78", 1, new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "José", "Soporte Técnico", 35000.00m, "809-555-0105" },
                    { 6, "Castro", "e.castro@organizacion.com", 1, "Av. Anacaona E12", 1, new DateTime(2015, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1983, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Elena", "Directora de RRHH", 95000.00m, "809-555-0106" },
                    { 7, "Santos", "r.santos@organizacion.com", 3, "Calle Las Carreras 203", 1, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1991, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Ricardo", "Supervisor de Despacho", 48000.00m, "809-555-0107" },
                    { 8, "Espinal", "l.espinal@organizacion.com", 1, "Urb. Real, Calle C", 1, new DateTime(2022, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1994, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Laura", "Analista de Reclutamiento", 40000.00m, "809-555-0108" },
                    { 9, "Jiménez", "p.jimenez@organizacion.com", 2, "Ensanche Luperón No. 5", 1, new DateTime(2018, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1989, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Pedro", "Administrador de Sistemas", 65000.00m, "809-555-0109" },
                    { 10, "Vargas", "s.vargas@organizacion.com", 3, "Villa Consuelo, Calle 3", 1, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1999, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "uploads/empleados/default.png", "Sofía", "Auxiliar de Almacén", 30000.00m, "809-555-0110" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_CorreoElectronico",
                table: "Empleados",
                column: "CorreoElectronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_DepartamentoId",
                table: "Empleados",
                column: "DepartamentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Departamentos");
        }
    }
}

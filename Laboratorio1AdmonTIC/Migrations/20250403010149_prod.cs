using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laboratorio1AdmonTIC.Migrations
{
    /// <inheritdoc />
    public partial class prod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Empleado",
            //    table: "Empleados");

            //migrationBuilder.RenameTable(
            //    name: "Empleado",
            //    newName: "Empleados");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Empleados",
            //    table: "Empleados",
            //    column: "EmpleadosId");

            //migrationBuilder.CreateTable(
            //    name: "Proveedores",
            //    columns: table => new
            //    {
            //        ProveedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Inactivo = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Proveedores", x => x.ProveedorId);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empleados",
                table: "Empleados");

            migrationBuilder.RenameTable(
                name: "Empleados",
                newName: "Empleados");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empleado",
                table: "Empleados",
                column: "EmpleadosId");
        }
    }
}

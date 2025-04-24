using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laboratorio1AdmonTIC.Migrations
{
    /// <inheritdoc />
    public partial class RelacionEmpleadosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Empleados",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Discriminator",
            //    table: "AspNetUsers",
            //    type: "nvarchar(21)",
            //    maxLength: 21,
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_UserId",
                table: "Empleados",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_AspNetUsers_UserId",
                table: "Empleados",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_AspNetUsers_UserId",
                table: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_UserId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Empleados");

            //migrationBuilder.DropColumn(
            //    name: "Discriminator",
            //    table: "AspNetUsers");
        }
    }
}

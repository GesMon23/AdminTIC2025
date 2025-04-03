using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laboratorio1AdmonTIC.Migrations
{
    /// <inheritdoc />
    public partial class munic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartamentoId",
                table: "Municipios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_DepartamentoId",
                table: "Municipios",
                column: "DepartamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Municipios_Departamento_DepartamentoId",
                table: "Municipios",
                column: "DepartamentoId",
                principalTable: "Departamento",
                principalColumn: "DepartamentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Municipios_Departamento_DepartamentoId",
                table: "Municipios");

            migrationBuilder.DropIndex(
                name: "IX_Municipios_DepartamentoId",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Municipios");
        }
    }
}

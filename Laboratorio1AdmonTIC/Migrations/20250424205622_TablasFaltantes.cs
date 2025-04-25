using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laboratorio1AdmonTIC.Migrations
{
    /// <inheritdoc />
    public partial class TablasFaltantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AspNetUsers_Empleados_EmpleadoId",
            //    table: "AspNetUsers");

            //migrationBuilder.DropIndex(
            //    name: "IX_AspNetUsers_EmpleadoId",
            //    table: "AspNetUsers");

            //migrationBuilder.DropColumn(
            //    name: "EmpleadoId",
            //    table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "EmpleadoId",
                table: "Clientes",
                newName: "ClienteId");

            migrationBuilder.AddColumn<bool>(
                name: "Inactivo",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AddColumn<string>(
            //    name: "UserId",
            //    table: "Empleados",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Discriminator",
            //    table: "AspNetUsers",
            //    type: "nvarchar(21)",
            //    maxLength: 21,
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    CompraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProveedoresProveedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpleadosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.CompraId);
                    table.ForeignKey(
                        name: "FK_Compras_Empleados_EmpleadosId",
                        column: x => x.EmpleadosId,
                        principalTable: "Empleados",
                        principalColumn: "EmpleadosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Compras_Proveedores_ProveedoresProveedorId",
                        column: x => x.ProveedoresProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventario",
                columns: table => new
                {
                    MovimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductosProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMovimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TiposMovimientoTipoMovimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpleadosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventario", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK_Inventario_Empleados_EmpleadosId",
                        column: x => x.EmpleadosId,
                        principalTable: "Empleados",
                        principalColumn: "EmpleadosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventario_Productos_ProductosProductoId",
                        column: x => x.ProductosProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventario_TiposMovimiento_TiposMovimientoTipoMovimientoId",
                        column: x => x.TiposMovimientoTipoMovimientoId,
                        principalTable: "TiposMovimiento",
                        principalColumn: "TipoMovimientoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    VentasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientesClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpleadosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetodoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetodosPagoMetodoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.VentasId);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClientesClienteId",
                        column: x => x.ClientesClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_Empleados_EmpleadosId",
                        column: x => x.EmpleadosId,
                        principalTable: "Empleados",
                        principalColumn: "EmpleadosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_MetodosPago_MetodosPagoMetodoId",
                        column: x => x.MetodosPagoMetodoId,
                        principalTable: "MetodosPago",
                        principalColumn: "MetodoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesCompras",
                columns: table => new
                {
                    DetalleCompraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesCompras", x => x.DetalleCompraId);
                    table.ForeignKey(
                        name: "FK_DetallesCompras_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesCompras_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesVenta",
                columns: table => new
                {
                    DetalleVentaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VentasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductosProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVenta", x => x.DetalleVentaId);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Productos_ProductosProductoId",
                        column: x => x.ProductosProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Ventas_VentasId",
                        column: x => x.VentasId,
                        principalTable: "Ventas",
                        principalColumn: "VentasId",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Empleados_UserId",
            //    table: "Empleados",
            //    column: "UserId",
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_EmpleadosId",
                table: "Compras",
                column: "EmpleadosId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ProveedoresProveedorId",
                table: "Compras",
                column: "ProveedoresProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompras_CompraId",
                table: "DetallesCompras",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompras_ProductoId",
                table: "DetallesCompras",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_ProductosProductoId",
                table: "DetallesVenta",
                column: "ProductosProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_VentasId",
                table: "DetallesVenta",
                column: "VentasId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_EmpleadosId",
                table: "Inventario",
                column: "EmpleadosId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_ProductosProductoId",
                table: "Inventario",
                column: "ProductosProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_TiposMovimientoTipoMovimientoId",
                table: "Inventario",
                column: "TiposMovimientoTipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClientesClienteId",
                table: "Ventas",
                column: "ClientesClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_EmpleadosId",
                table: "Ventas",
                column: "EmpleadosId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_MetodosPagoMetodoId",
                table: "Ventas",
                column: "MetodosPagoMetodoId");

            //migrationBuilder.AddForeignKey(
                //name: "FK_Empleados_AspNetUsers_UserId",
                //table: "Empleados",
                //column: "UserId",
                //principalTable: "AspNetUsers",
                //principalColumn: "Id",
                //onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_AspNetUsers_UserId",
                table: "Empleados");

            migrationBuilder.DropTable(
                name: "DetallesCompras");

            migrationBuilder.DropTable(
                name: "DetallesVenta");

            migrationBuilder.DropTable(
                name: "Inventario");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_UserId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "Inactivo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Clientes",
                newName: "EmpleadoId");

            migrationBuilder.AddColumn<Guid>(
                name: "EmpleadoId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmpleadoId",
                table: "AspNetUsers",
                column: "EmpleadoId",
                unique: true,
                filter: "[EmpleadoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empleados_EmpleadoId",
                table: "AspNetUsers",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "EmpleadosId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

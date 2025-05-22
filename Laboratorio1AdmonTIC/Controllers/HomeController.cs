using Laboratorio1AdmonTIC.Models;
using Laboratorio1AdmonTIC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using static Laboratorio1AdmonTIC.ViewModels.EstadisticasViewModel;
using OfficeOpenXml.Table;


namespace Laboratorio1AdmonTIC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ERPDbContext _context;

        public HomeController(ILogger<HomeController> logger, ERPDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var tipoCompraGuid = Guid.Parse("9E1EA92E-D21A-4E54-9A44-40947ECFFB5A");

            var movimientos = await (
                from i in _context.Inventario
                join c in _context.DetallesCompras on i.CompraId equals c.CompraId into comprasJoin
                from c in comprasJoin.DefaultIfEmpty()
                join v in _context.DetallesVenta on i.VentaId equals v.VentasId into ventasJoin
                from v in ventasJoin.DefaultIfEmpty()
                where i.FechaCompra.Date == hoy && !i.Inactivo
                orderby i.FechaCompra descending
                select new MovimientoRecienteDto
                {
                    TipoMovimiento = i.TipoMovimientoId == tipoCompraGuid ? "Compra" : "Venta",
                    Total = i.TipoMovimientoId == tipoCompraGuid
                        ? (c != null ? c.Total : 0)
                        : (v != null ? v.Total : 0),
                    Fecha = i.FechaCompra
                }
            )
            .Take(5)
            .ToListAsync();

            var modelo = new DashboardViewModel
            {
                // Total de compras hoy
                TotalComprasHoy = await _context.Compras
                    .Where(c => c.FechaCompra.Date == hoy && !c.Inactivo)
                    .SumAsync(c => (decimal?)c.Total) ?? 0,

                // Total de ventas hoy
                TotalVentasHoy = await _context.Ventas
                    .Where(v => v.FechaVenta.Date == hoy && !v.Inactivo)
                    .SumAsync(v => (decimal?)v.Total) ?? 0,

                // Productos con stock
                TotalStockDisponible = await _context.Productos
                    .Where(p => p.Stock > 0 && !p.Inactivo)
                    .CountAsync(),

                // Productos sin stock
                TotalStockAgotado = await _context.Productos
                    .Where(p => p.Stock <= 0 && !p.Inactivo)
                    .CountAsync(),

                // Productos en stock mínimo
                ProductosStockMinimo = await _context.Productos
                    .Where(p => p.Stock <= p.StockMinimo && !p.Inactivo)
                    .Select(p => new ProductoStockMinimoDto
                    {
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Stock = p.Stock,
                        StockMinimo = p.StockMinimo
                    })
                    .ToListAsync(),

                // Movimientos recientes (LEFT JOIN)
                MovimientosRecientes = movimientos
            };

            return View(modelo);
        }



        public async Task<IActionResult> Privacy()
        {
            var hoy = DateTime.Today;
            var TotalMesActual = await _context.Ventas
                                 .Where(v => v.FechaVenta.Month == hoy.Month && v.FechaVenta.Year == hoy.Year && !v.Inactivo)
                                 .SumAsync(v => (decimal?)v.Total) ?? 0;

            var fechaMesPasado = DateTime.Today.AddMonths(-1);
            var totalVentasMesPasado = await _context.Ventas
                .Where(v => v.FechaVenta.Month == fechaMesPasado.Month && v.FechaVenta.Year == fechaMesPasado.Year && !v.Inactivo)
                .SumAsync(v => (decimal?)v.Total) ?? 0;

            decimal PorcentajeDiferencia = 0;
            if (totalVentasMesPasado == 0)
            {
                PorcentajeDiferencia = 0;
            }
            else
            {
                PorcentajeDiferencia = ((TotalMesActual - totalVentasMesPasado) / totalVentasMesPasado) * 100;
            }

            var unidades = await (
                                from dv in _context.DetallesVenta
                                join v in _context.Ventas on dv.VentasId equals v.VentasId
                                where v.FechaVenta.Month == hoy.Month && v.FechaVenta.Year == hoy.Year && !dv.Inactivo
                                select (int?)dv.Cantidad
                            ).SumAsync() ?? 0;
            var difUnidades = await (
                                from dv in _context.DetallesVenta
                                join v in _context.Ventas on dv.VentasId equals v.VentasId
                                where v.FechaVenta.Month == fechaMesPasado.Month && v.FechaVenta.Year == fechaMesPasado.Year && !dv.Inactivo
                                select (int?)dv.Cantidad
                            ).SumAsync() ?? 0;
            decimal diferenciaUvendidas = 0;
            if (difUnidades == 0)
            {
                diferenciaUvendidas = 0;
            }
            else
            {
                diferenciaUvendidas = ((unidades - (decimal)difUnidades) / difUnidades) * 100;
            }

            var numVentas = await _context.Ventas
                .Where(v => v.FechaVenta.Month == hoy.Month && v.FechaVenta.Year == hoy.Year && !v.Inactivo)
                .CountAsync();
            var numVentasA = await _context.Ventas
                .Where(v => v.FechaVenta.Month == fechaMesPasado.Month && v.FechaVenta.Year == fechaMesPasado.Year && !v.Inactivo)
                .CountAsync();
            decimal difnumVentas = 0;
            if (numVentasA == 0)
            {
                difnumVentas = 0;
            }
            else
            {
                difnumVentas = ((numVentas - (decimal)numVentasA) / numVentasA) * 100;
            }


            var masVendido = new EstadisticasViewModel.MasVendidoViewModel
            {
                empleado = (await (
                         from v in _context.Ventas
                         join e in _context.Empleados on v.EmpleadosId equals e.EmpleadosId
                         where v.FechaVenta.Month == hoy.Month &&
                               v.FechaVenta.Year == hoy.Year &&
                               !v.Inactivo
                         group v by new { e.EmpleadosId, e.Nombres } into g
                         orderby g.Sum(x => x.Total) descending
                         select g.Key.Nombres
                     ).FirstOrDefaultAsync()) ?? "No hay empleados",

                productoVendido = (await (
                         from v in _context.Ventas
                         join dv in _context.DetallesVenta on v.VentasId equals dv.VentasId
                         join p in _context.Productos on dv.ProductoId equals p.ProductoId
                         where v.FechaVenta.Month == hoy.Month &&
                               v.FechaVenta.Year == hoy.Year &&
                               !v.Inactivo
                         group dv by new { p.ProductoId, p.Nombre } into g
                         orderby g.Sum(x => x.Cantidad) descending
                         select g.Key.Nombre
                     ).FirstOrDefaultAsync()) ?? "No hay productos"
            };


            var modelo = new EstadisticasViewModel
            {
                totalVentas = Math.Round(TotalMesActual, 2),
                difVentas = Math.Round(PorcentajeDiferencia, 2),
                unidadesVendidas = unidades,
                difUvendidas = Math.Round(diferenciaUvendidas, 2),
                numeroVentas = numVentas,
                difNumeroVentas = Math.Round(difnumVentas, 2),
                MasVendido = masVendido
            };
            return View(modelo);

        }

        [HttpGet]
        public async Task<IActionResult> GetVentasPorMes()
        {
            var datos = await (
                from v in _context.Ventas
                group v by new { v.FechaVenta.Year, v.FechaVenta.Month } into g
                orderby g.Key.Year, g.Key.Month
                select new
                {
                    anio = g.Key.Year,
                    mes = g.Key.Month,
                    ventasDelMes = g.Count()
                }
            ).ToListAsync();

            return Json(datos);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductoMasVendido()
        {
            var hoy = DateTime.Now;

            var datos = await (
                from v in _context.Ventas
                join dv in _context.DetallesVenta on v.VentasId equals dv.VentasId
                join p in _context.Productos on dv.ProductoId equals p.ProductoId
                where v.FechaVenta.Month == hoy.Month &&
                      v.FechaVenta.Year == hoy.Year
                group dv by new { p.ProductoId, p.Nombre, v.FechaVenta.Year, v.FechaVenta.Month } into g
                where g.Sum(x => x.Cantidad) > 10
                orderby g.Sum(x => x.Cantidad) descending
                select new
                {
                    nombre = g.Key.Nombre,
                    cantidad = g.Sum(x => x.Cantidad),
                    mes = g.Key.Month,
                    anio = g.Key.Year
                }
            ).ToListAsync();

            return Json(datos);
        }

        [HttpGet]
        public async Task<IActionResult> GetPorcentajeUsoMetodosPago()
        {
            var grupo = await (
                from v in _context.Ventas
                join mp in _context.MetodosPago on v.MetodoId equals mp.MetodoId
                where !v.Inactivo
                group v by new { mp.MetodoId, mp.TipoPago } into g
                select new
                {
                    Metodo = g.Key.MetodoId,
                    TPago = g.Key.TipoPago,
                    Cantidad = g.Count()
                }
            ).ToListAsync();

            var total = grupo.Sum(x => x.Cantidad);

            // Calculamos el porcentaje sobre el total
            var resultado = grupo
                .Select(x => new
                {
                    x.TPago,
                    PorcentajeUso = Math.Round((x.Cantidad * 100.0) / total, 2)
                })
                .OrderByDescending(x => x.PorcentajeUso)
                .ToList();

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> GetComparativaVC()
        {
            // Agrupamos compras por año y mes
            var compras = await (
                from c in _context.Compras
                where !c.Inactivo
                group c by new { c.FechaCompra.Year, c.FechaCompra.Month } into g
                select new
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    TotalCompras = g.Sum(x => x.Total)
                }
            ).ToListAsync();

            // Agrupamos ventas por año y mes
            var ventas = await (
                from v in _context.Ventas
                where !v.Inactivo
                group v by new { v.FechaVenta.Year, v.FechaVenta.Month } into g
                select new
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    TotalVentas = g.Sum(x => x.Total)
                }
            ).ToListAsync();

            // Obtener todos los pares únicos de año/mes de ambas listas
            var fechasUnicas = compras
                .Select(c => new { c.Anio, c.Mes })
                .Union(ventas.Select(v => new { v.Anio, v.Mes }))
                .Distinct();

            // Combinar en un solo resultado
            var resultado = fechasUnicas
                .Select(f => new
                {
                    Aniog = f.Anio,
                    Mesg = f.Mes,
                    TotalCompras = compras.FirstOrDefault(c => c.Anio == f.Anio && c.Mes == f.Mes)?.TotalCompras ?? 0,
                    TotalVentas = ventas.FirstOrDefault(v => v.Anio == f.Anio && v.Mes == f.Mes)?.TotalVentas ?? 0
                })
                .OrderBy(r => r.Aniog)
                .ThenBy(r => r.Mesg)
                .ToList();

            return Json(resultado);
        }




        [HttpGet]
        public async Task<IActionResult> ExportarVentasExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            var ventas = await (from v in _context.Ventas
                                join dv in _context.DetallesVenta on v.VentasId equals dv.VentasId
                                join p in _context.Productos on dv.ProductoId equals p.ProductoId
                                join e in _context.Empleados on v.EmpleadosId equals e.EmpleadosId
                                join c in _context.Clientes on v.ClienteId equals c.ClienteId
                                where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin && !v.Inactivo
                                orderby v.FechaVenta descending
                                select new
                                {
                                    Fecha = v.FechaVenta,
                                    Producto = p.Nombre,
                                    Cant = dv.Cantidad,
                                    Precio = dv.PrecioUnitario,
                                    Total = dv.Cantidad * dv.PrecioUnitario
                                }).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reporte de Ventas");

            // Encabezados
            ws.Cells[1, 1].Value = "Fecha";
            ws.Cells[1, 2].Value = "Producto";
            ws.Cells[1, 3].Value = "Cantidad";
            ws.Cells[1, 4].Value = "Precio";
            ws.Cells[1, 5].Value = "Total";

            int row = 2;
            foreach (var v in ventas)
            {
                ws.Cells[row, 1].Value = v.Fecha.ToString("yyyy-MM-dd");
                ws.Cells[row, 2].Value = v.Producto;
                ws.Cells[row, 3].Value = v.Cant;
                ws.Cells[row, 4].Value = v.Precio;
                ws.Cells[row, 5].Value = v.Total;
                row++;
            }

            // Ajustar tamaño de columnas
            ws.Cells.AutoFitColumns();

            // Crear una tabla desde A1 hasta E{row - 1}
            var tblRange = ws.Cells[1, 1, row - 1, 5];
            var table = ws.Tables.Add(tblRange, "TablaVentas");
            table.TableStyle = TableStyles.Medium14;
           

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteVentas.xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> ReporteCompras(DateTime fechaInicio, DateTime fechaFin)
        {
            var compras = await (from c in _context.Compras
                                join p in _context.Proveedores on c.ProveedorId equals p.ProveedorId
                                join dc in _context.DetallesCompras on c.CompraId equals dc.CompraId
                                join pc in _context.Productos on dc.ProductoId equals pc.ProductoId
                                where c.FechaCompra >= fechaInicio && c.FechaCompra <= fechaFin && !c.Inactivo
                                orderby c.FechaCompra descending
                                select new
                                {
                                    Fecha = c.FechaCompra,
                                    proveedor = p.Nombre,
                                    producto = pc.Nombre,
                                    cantidad = dc.Cantidad,
                                    precio = pc.PrecioCompra,
                                    total = c.Total
                                }).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reporte de Compras");

            // Encabezados
            ws.Cells[1, 1].Value = "Fecha";
            ws.Cells[1, 2].Value = "Proveedor";
            ws.Cells[1, 3].Value = "Producto";
            ws.Cells[1, 4].Value = "Cantidad";
            ws.Cells[1, 5].Value = "Precio de Compra";
            ws.Cells[1, 6].Value = "Total";

            int row = 2;
            foreach (var c in compras)
            {
                ws.Cells[row, 1].Value = c.Fecha.ToString("yyyy-MM-dd");
                ws.Cells[row, 2].Value = c.proveedor;
                ws.Cells[row, 3].Value = c.producto;
                ws.Cells[row, 4].Value = c.cantidad;
                ws.Cells[row, 5].Value = c.precio;
                ws.Cells[row, 6].Value = c.total;
                row++;
            }

            // Ajustar tamaño de columnas
            ws.Cells.AutoFitColumns();

            // Crear una tabla desde A1 hasta E{row - 1}
            var tblRange = ws.Cells[1, 1, row - 1, 6];
            var table = ws.Tables.Add(tblRange, "TablaCompras");
            table.TableStyle = TableStyles.Medium14;


            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteCompras.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ReporteInventario()
        {
            var inventario = await (from p in _context.Productos
                                 where !p.Inactivo
                                 orderby p.Nombre descending
                                 select new
                                 {
                                     nombre = p.Nombre,
                                     stock = p.Stock,
                                     stockMinimo = p.StockMinimo,
                                     precioC = p.PrecioCompra,
                                     total = (p.Stock * p.PrecioCompra),
                                     precioV = p.PrecioVenta
                                 }).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reporte de Compras");

            // Encabezados
            ws.Cells[1, 1].Value = "Producto";
            ws.Cells[1, 2].Value = "Stock";
            ws.Cells[1, 3].Value = "Stock Minímo";
            ws.Cells[1, 4].Value = "Precio de Compra";
            ws.Cells[1, 5].Value = "Total";
            ws.Cells[1, 6].Value = "Precio de Venta";

            int row = 2;
            foreach (var i in inventario)
            {
                ws.Cells[row, 1].Value = i.nombre;
                ws.Cells[row, 2].Value = i.stock;
                ws.Cells[row, 3].Value = i.stockMinimo;
                ws.Cells[row, 4].Value = i.precioC;
                ws.Cells[row, 5].Value = i.total;
                ws.Cells[row, 6].Value = i.precioV;
                row++;
            }

            // Ajustar tamaño de columnas
            ws.Cells.AutoFitColumns();

            // Crear una tabla desde A1 hasta E{row - 1}
            var tblRange = ws.Cells[1, 1, row - 1, 6];
            var table = ws.Tables.Add(tblRange, "TablaInventario");
            table.TableStyle = TableStyles.Medium14;


            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteInventario.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ReporteMovimientos(DateTime fechaInicio, DateTime fechaFin, string movimiento)
        {
            var control = await (from i in _context.Inventario
                                 join tm in _context.TiposMovimiento on i.TipoMovimientoId equals tm.TipoMovimientoId
                                 join p in _context.Productos on i.ProductoId equals p.ProductoId
                                 join e in _context.Empleados on i.EmpleadosId equals e.EmpleadosId
                                 where i.FechaCompra >= fechaInicio && i.FechaCompra <= fechaFin && !i.Inactivo && (movimiento == null || movimiento == "" || tm.TipoMovimiento == movimiento)
                                 orderby i.FechaCompra descending
                                 select new
                                 {
                                     Fecha = i.FechaCompra,
                                     tipoMovimiento = tm.TipoMovimiento,
                                     producto = p.Nombre,
                                     cantidad = i.Cantidad,
                                     empleado = (e.Nombres + ' ' + e.Apellidos)
                                 }).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reporte de Movimientos de Inventario");

            // Encabezados
            ws.Cells[1, 1].Value = "Fecha";
            ws.Cells[1, 2].Value = "Movimiento";
            ws.Cells[1, 3].Value = "Producto";
            ws.Cells[1, 4].Value = "Cantidad";
            ws.Cells[1, 5].Value = "Empleado";

            int row = 2;
            foreach (var c in control)
            {
                ws.Cells[row, 1].Value = c.Fecha.ToString("yyyy-MM-dd");
                ws.Cells[row, 2].Value = c.tipoMovimiento;
                ws.Cells[row, 3].Value = c.producto;
                ws.Cells[row, 4].Value = c.cantidad;
                ws.Cells[row, 5].Value = c.empleado;
                row++;
            }

            // Ajustar tamaño de columnas
            ws.Cells.AutoFitColumns();

            // Crear una tabla desde A1 hasta E{row - 1}
            var tblRange = ws.Cells[1, 1, row - 1, 5];
            var table = ws.Tables.Add(tblRange, "TablaMovimientos");
            table.TableStyle = TableStyles.Medium14;


            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteMovimientos.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ReporteBajoStock()
        {
            var bajoS = await (from p in _context.Productos
                                    where !p.Inactivo && p.Stock <= p.StockMinimo
                                    orderby p.Nombre descending
                                    select new
                                    {
                                        nombre = p.Nombre,
                                        stock = p.Stock,
                                        stockMinimo = p.StockMinimo,
                                        precioC = p.PrecioCompra,
                                        precioV = p.PrecioVenta
                                    }).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reporte de Bajo Stock");

            // Encabezados
            ws.Cells[1, 1].Value = "Producto";
            ws.Cells[1, 2].Value = "Stock";
            ws.Cells[1, 3].Value = "Stock Minímo";
            ws.Cells[1, 4].Value = "Precio de Compra";
            ws.Cells[1, 5].Value = "Precio de Venta";

            int row = 2;
            foreach (var b in bajoS)
            {
                ws.Cells[row, 1].Value = b.nombre;
                ws.Cells[row, 2].Value = b.stock;
                ws.Cells[row, 3].Value = b.stockMinimo;
                ws.Cells[row, 4].Value = b.precioC;
                ws.Cells[row, 5].Value = b.precioV;
                row++;
            }

            // Ajustar tamaño de columnas
            ws.Cells.AutoFitColumns();

            // Crear una tabla desde A1 hasta E{row - 1}
            var tblRange = ws.Cells[1, 1, row - 1, 5];
            var table = ws.Tables.Add(tblRange, "TablaBajoStock");
            table.TableStyle = TableStyles.Medium14;


            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteBajoStock.xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> ReporteRentabilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            var rentabilidad = await (from dv in _context.DetallesVenta
                                      join p in _context.Productos on dv.ProductoId equals p.ProductoId
                                      join v in _context.Ventas on dv.VentasId equals v.VentasId
                                      where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin && !dv.Inactivo
                                      group new { dv, p } by new { p.Nombre, p.PrecioCompra, p.PrecioVenta } into g
                                      select new
                                      {
                                          Producto = g.Key.Nombre,
                                          UnidadesVendidas = g.Sum(x => x.dv.Cantidad),
                                          PrecioCompra = g.Key.PrecioCompra,
                                          PrecioVenta = g.Key.PrecioVenta,
                                          Ganancia = (g.Key.PrecioVenta - g.Key.PrecioCompra) * g.Sum(x => x.dv.Cantidad)
                                      }).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reporte de Rentabilidad");

            // Encabezados
            ws.Cells[1, 1].Value = "Producto";
            ws.Cells[1, 2].Value = "Unidades Vendidas";
            ws.Cells[1, 3].Value = "Precio Compra";
            ws.Cells[1, 4].Value = "Precio Venta";
            ws.Cells[1, 5].Value = "Ganancia";

            int row = 2;
            foreach (var r in rentabilidad)
            {
                ws.Cells[row, 1].Value = r.Producto;
                ws.Cells[row, 2].Value = r.UnidadesVendidas;
                ws.Cells[row, 3].Value = r.PrecioCompra;
                ws.Cells[row, 4].Value = r.PrecioVenta;
                ws.Cells[row, 5].Value = r.Ganancia;
                row++;
            }

            ws.Cells.AutoFitColumns();

            var tblRange = ws.Cells[1, 1, row - 1, 5];
            var table = ws.Tables.Add(tblRange, "TablaRentabilidad");
            table.TableStyle = TableStyles.Medium14;

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteRentabilidad.xlsx");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

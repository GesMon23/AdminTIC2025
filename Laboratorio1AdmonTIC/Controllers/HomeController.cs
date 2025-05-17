using Laboratorio1AdmonTIC.Models;
using Laboratorio1AdmonTIC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
                join c in _context.Compras on i.CompraId equals c.CompraId into comprasJoin
                from c in comprasJoin.DefaultIfEmpty()
                join v in _context.Ventas on i.VentaId equals v.VentasId into ventasJoin
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



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

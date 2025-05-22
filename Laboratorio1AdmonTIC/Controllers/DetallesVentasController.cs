using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Laboratorio1AdmonTIC.Models;
using Laboratorio1AdmonTIC.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Laboratorio1AdmonTIC.Controllers
{
	[Authorize]
	public class DetallesVentasController : Controller
    {
        private readonly ERPDbContext _context;

        public DetallesVentasController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: DetallesVentas
        public async Task<IActionResult> Index()
        {
            //return View(await _context.DetallesVenta.Where(c => !c.Inactivo).ToListAsync());
            var dventas = await (from dv in _context.DetallesVenta
                                 join p in _context.Productos on dv.ProductoId equals p.ProductoId
                                 where !dv.Inactivo
                                 select new DetallesVentasViewModel 
                                 {
                                     DetalleVentaId= dv.DetalleVentaId,
                                     VentasId = dv.VentasId,
                                     Producto = p.Nombre,
                                     Cantidad = dv.Cantidad,
                                     PrecioUnitario = dv.PrecioUnitario,
                                     Total = dv.Total
                                 }).ToListAsync();
            return View(dventas);
        }

        // GET: DetallesVentas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVenta = await _context.DetallesVenta
                //.Include(d => d.Ventas)
                .FirstOrDefaultAsync(m => m.DetalleVentaId == id);
            if (detallesVenta == null)
            {
                return NotFound();
            }

            return View(detallesVenta);
        }

        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
            var detallesVentas = (from dc in _context.DetallesVenta
                                   join p in _context.Productos on dc.ProductoId equals p.ProductoId
                                   where !dc.Inactivo && dc.DetalleVentaId == id
                                   select new DetallesVentasViewModel
                                   {
                                       DetalleVentaId = dc.DetalleVentaId,
                                       VentasId = dc.VentasId,
                                       Producto = p.Nombre,
                                       Descripcion = p.Descripcion,
                                       Cantidad = dc.Cantidad,
                                       PrecioUnitario = dc.PrecioUnitario,
                                       Total = dc.Total
                                   }).FirstOrDefault();

            if (detallesVentas == null)
            {
                return NotFound();
            }

            return PartialView("Details", detallesVentas);
        }

        // GET: DetallesVentas/Create
        public IActionResult Create()
        {
            ViewBag.Ventas = new SelectList(_context.Ventas.Where(p => !p.Inactivo), "VentasId", "VentasId");
            ViewBag.Productos = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
            return View();
        }

        // POST: DetallesVentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetalleVentaId,VentasId,ProductoId,Cantidad,PrecioUnitario,Total")] DetallesVenta detallesVenta)
        {
            if (ModelState.IsValid)
            {
                detallesVenta.DetalleVentaId = Guid.NewGuid();
                _context.Add(detallesVenta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Ventas = new SelectList(_context.Ventas.Where(p => !p.Inactivo), "VentasId", "VentasId");
            ViewBag.Productos = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
            return View(detallesVenta);
        }

        // GET: DetallesVentas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVenta = await _context.DetallesVenta.FindAsync(id);
            if (detallesVenta == null)
            {
                return NotFound();
            }
            ViewBag.Ventas = new SelectList(_context.Ventas.Where(p => !p.Inactivo), "VentasId", "VentasId");
            ViewBag.Productos = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
            return View(detallesVenta);
        }

        // POST: DetallesVentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DetalleVentaId,VentasId,ProductoId,Cantidad,PrecioUnitario,Total")] DetallesVenta detallesVenta)
        {
            if (id != detallesVenta.DetalleVentaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallesVenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallesVentaExists(detallesVenta.DetalleVentaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Ventas = new SelectList(_context.Ventas.Where(p => !p.Inactivo), "VentasId", "VentasId");
            ViewBag.Productos = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
            return View(detallesVenta);
        }

        // GET: DetallesVentas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVenta = await _context.DetallesVenta
                //.Include(d => d.Ventas)
                .FirstOrDefaultAsync(m => m.DetalleVentaId == id);
            if (detallesVenta == null)
            {
                return NotFound();
            }

            //return View(detallesVenta);
            detallesVenta.Inactivo = true;
            _context.Update(detallesVenta);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: DetallesVentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var detallesVenta = await _context.DetallesVenta.FindAsync(id);
            if (detallesVenta != null)
            {
                _context.DetallesVenta.Remove(detallesVenta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallesVentaExists(Guid id)
        {
            return _context.DetallesVenta.Any(e => e.DetalleVentaId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Laboratorio1AdmonTIC.Models;

namespace Laboratorio1AdmonTIC.Controllers
{
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
            var eRPDbContext = _context.DetallesVenta.Include(d => d.Ventas);
            return View(await eRPDbContext.ToListAsync());
        }

        // GET: DetallesVentas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVenta = await _context.DetallesVenta
                .Include(d => d.Ventas)
                .FirstOrDefaultAsync(m => m.DetalleVentaId == id);
            if (detallesVenta == null)
            {
                return NotFound();
            }

            return View(detallesVenta);
        }

        // GET: DetallesVentas/Create
        public IActionResult Create()
        {
            ViewData["VentasId"] = new SelectList(_context.Ventas, "VentasId", "VentasId");
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
            ViewData["VentasId"] = new SelectList(_context.Ventas, "VentasId", "VentasId", detallesVenta.VentasId);
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
            ViewData["VentasId"] = new SelectList(_context.Ventas, "VentasId", "VentasId", detallesVenta.VentasId);
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
            ViewData["VentasId"] = new SelectList(_context.Ventas, "VentasId", "VentasId", detallesVenta.VentasId);
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
                .Include(d => d.Ventas)
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

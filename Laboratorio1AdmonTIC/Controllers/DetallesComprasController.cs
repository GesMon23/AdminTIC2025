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
    public class DetallesComprasController : Controller
    {
        private readonly ERPDbContext _context;

        public DetallesComprasController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: DetallesCompras
        public async Task<IActionResult> Index()
        {
            return View(await _context.DetallesCompras.ToListAsync());
        }

        // GET: DetallesCompras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesCompras = await _context.DetallesCompras
                .FirstOrDefaultAsync(m => m.DetalleCompraId == id);
            if (detallesCompras == null)
            {
                return NotFound();
            }

            return View(detallesCompras);
        }

        // GET: DetallesCompras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DetallesCompras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetalleCompraId,CompraId,ProductoId,Cantidad,PrecioUnitario,Total")] DetallesCompras detallesCompras)
        {
            if (ModelState.IsValid)
            {
                detallesCompras.DetalleCompraId = Guid.NewGuid();
                _context.Add(detallesCompras);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(detallesCompras);
        }

        // GET: DetallesCompras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesCompras = await _context.DetallesCompras.FindAsync(id);
            if (detallesCompras == null)
            {
                return NotFound();
            }
            return View(detallesCompras);
        }

        // POST: DetallesCompras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DetalleCompraId,CompraId,ProductoId,Cantidad,PrecioUnitario,Total")] DetallesCompras detallesCompras)
        {
            if (id != detallesCompras.DetalleCompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallesCompras);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallesComprasExists(detallesCompras.DetalleCompraId))
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
            return View(detallesCompras);
        }

        // GET: DetallesCompras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesCompras = await _context.DetallesCompras
                .FirstOrDefaultAsync(m => m.DetalleCompraId == id);
            if (detallesCompras == null)
            {
                return NotFound();
            }

            //return View(detallesCompras);
            detallesCompras.Inactivo = true;
            _context.Update(detallesCompras);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: DetallesCompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var detallesCompras = await _context.DetallesCompras.FindAsync(id);
            if (detallesCompras != null)
            {
                _context.DetallesCompras.Remove(detallesCompras);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallesComprasExists(Guid id)
        {
            return _context.DetallesCompras.Any(e => e.DetalleCompraId == id);
        }
    }
}

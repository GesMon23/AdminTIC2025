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
    public class ComprasController : Controller
    {
        private readonly ERPDbContext _context;

        public ComprasController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var eRPDbContext = _context.Compras.Include(c => c.Empleados);
            return View(await eRPDbContext.ToListAsync());
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras
                .Include(c => c.Empleados)
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compras == null)
            {
                return NotFound();
            }

            return View(compras);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId");
            return View();
        }

        // POST: Compras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompraId,ProveedorId,EmpleadosId,FechaCompra,Total")] Compras compras)
        {
            if (ModelState.IsValid)
            {
                compras.CompraId = Guid.NewGuid();
                _context.Add(compras);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId", compras.EmpleadosId);
            return View(compras);
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras.FindAsync(id);
            if (compras == null)
            {
                return NotFound();
            }
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId", compras.EmpleadosId);
            return View(compras);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompraId,ProveedorId,EmpleadosId,FechaCompra,Total")] Compras compras)
        {
            if (id != compras.CompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compras);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComprasExists(compras.CompraId))
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
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId", compras.EmpleadosId);
            return View(compras);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras
                .Include(c => c.Empleados)
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compras == null)
            {
                return NotFound();
            }

            //return View(compras);
            compras.Inactivo = true;
            _context.Update(compras);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var compras = await _context.Compras.FindAsync(id);
            if (compras != null)
            {
                _context.Compras.Remove(compras);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComprasExists(Guid id)
        {
            return _context.Compras.Any(e => e.CompraId == id);
        }
    }
}

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
    public class InventariosController : Controller
    {
        private readonly ERPDbContext _context;

        public InventariosController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Inventarios
        public async Task<IActionResult> Index()
        {
            var eRPDbContext = _context.Inventario.Include(i => i.Empleados);
            return View(await eRPDbContext.ToListAsync());
        }

        // GET: Inventarios/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .Include(i => i.Empleados)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // GET: Inventarios/Create
        public IActionResult Create()
        {
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId");
            return View();
        }

        // POST: Inventarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovimientoId,ProductoId,TipoMovimientoId,EmpleadosId,Cantidad,FechaCompra")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                inventario.MovimientoId = Guid.NewGuid();
                _context.Add(inventario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId", inventario.EmpleadosId);
            return View(inventario);
        }

        // GET: Inventarios/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId", inventario.EmpleadosId);
            return View(inventario);
        }

        // POST: Inventarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MovimientoId,ProductoId,TipoMovimientoId,EmpleadosId,Cantidad,FechaCompra")] Inventario inventario)
        {
            if (id != inventario.MovimientoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventarioExists(inventario.MovimientoId))
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
            ViewData["EmpleadosId"] = new SelectList(_context.Empleados, "EmpleadosId", "EmpleadosId", inventario.EmpleadosId);
            return View(inventario);
        }

        // GET: Inventarios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .Include(i => i.Empleados)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
            if (inventario == null)
            {
                return NotFound();
            }

            //return View(inventario);
            inventario.Inactivo = true;
            _context.Update(inventario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Inventarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventario.Remove(inventario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(Guid id)
        {
            return _context.Inventario.Any(e => e.MovimientoId == id);
        }
    }
}

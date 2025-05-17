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
    public class TiposMovimientoesController : Controller
    {
        private readonly ERPDbContext _context;

        public TiposMovimientoesController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: TiposMovimientoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposMovimiento.Where(c => !c.Inactivo).ToListAsync());
        }

        // GET: TiposMovimientoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposMovimiento = await _context.TiposMovimiento
                .FirstOrDefaultAsync(m => m.TipoMovimientoId == id);
            if (tiposMovimiento == null)
            {
                return NotFound();
            }

            return PartialView("Details", tiposMovimiento);
        }

        // GET: TiposMovimientoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoMovimientoId,TipoMovimiento")] TiposMovimiento tiposMovimiento)
        {
            if (ModelState.IsValid)
            {
                tiposMovimiento.TipoMovimientoId = Guid.NewGuid();
                _context.Add(tiposMovimiento);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tipo de Movimiento registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Ocurrio un error al guardar.";
            return RedirectToAction(nameof(Create));
        }

        // GET: TiposMovimientoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposMovimiento = await _context.TiposMovimiento.FindAsync(id);
            if (tiposMovimiento == null)
            {
                return NotFound();
            }
            return View(tiposMovimiento);
        }

        // POST: TiposMovimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TipoMovimientoId,TipoMovimiento")] TiposMovimiento tiposMovimiento)
        {
            if (id != tiposMovimiento.TipoMovimientoId)
            {
                TempData["Error"] = "Ocurrio un error al actualizar.";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tiposMovimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TiposMovimientoExists(tiposMovimiento.TipoMovimientoId))
                    {
                        TempData["Error"] = "Ocurrio un error al actualizar.";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Tipo de movimiento correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Modelo invalido (faltan datos).";
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // GET: TiposMovimientoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id no válido.";
                return RedirectToAction(nameof(Index));
            }

            var tiposMovimiento = await _context.TiposMovimiento
                .FirstOrDefaultAsync(m => m.TipoMovimientoId == id);
            if (tiposMovimiento == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //return View(tiposMovimiento);
            tiposMovimiento.Inactivo = true;
            _context.Update(tiposMovimiento);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registro eliminado correctamente.";
            return RedirectToAction("Index");
        }

        // POST: TiposMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tiposMovimiento = await _context.TiposMovimiento.FindAsync(id);
            if (tiposMovimiento != null)
            {
                _context.TiposMovimiento.Remove(tiposMovimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TiposMovimientoExists(Guid id)
        {
            return _context.TiposMovimiento.Any(e => e.TipoMovimientoId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Laboratorio1AdmonTIC.Models;
using Microsoft.AspNetCore.Authorization;

namespace Laboratorio1AdmonTIC.Controllers
{
	[Authorize]
	public class MetodosPagoesController : Controller
    {
        private readonly ERPDbContext _context;

        public MetodosPagoesController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: MetodosPagoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.MetodosPago.Where(c => !c.Inactivo).ToListAsync());
        }

        // GET: MetodosPagoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metodosPago = await _context.MetodosPago
                .FirstOrDefaultAsync(m => m.MetodoId == id);
            if (metodosPago == null)
            {
                return NotFound();
            }

            return PartialView("Details", metodosPago);
        }

        // GET: MetodosPagoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MetodosPagoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MetodoId,TipoPago")] MetodosPago metodosPago)
        {
            if (ModelState.IsValid)
            {
                metodosPago.MetodoId = Guid.NewGuid();
                _context.Add(metodosPago);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Metodo de Pago registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Ocurrio un error al guardar.";
            return RedirectToAction(nameof(Create));
        }

        // GET: MetodosPagoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metodosPago = await _context.MetodosPago.FindAsync(id);
            if (metodosPago == null)
            {
                return NotFound();
            }
            return View(metodosPago);
        }

        // POST: MetodosPagoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MetodoId,TipoPago")] MetodosPago metodosPago)
        {
            if (id != metodosPago.MetodoId)
            {
                TempData["Error"] = "Ocurrio un error al actualizar.";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(metodosPago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MetodosPagoExists(metodosPago.MetodoId))
                    {
                        TempData["Error"] = "Ocurrio un error al actualizar.";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Metodo de pago actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Modelo invalido (faltan datos).";
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // GET: MetodosPagoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id no válido.";
                return RedirectToAction(nameof(Index));
            }

            var metodosPago = await _context.MetodosPago
                .FirstOrDefaultAsync(m => m.MetodoId == id);
            if (metodosPago == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //return View(metodosPago);
            metodosPago.Inactivo = true;
            _context.Update(metodosPago);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registro eliminado correctamente.";
            return RedirectToAction("Index");
        }

        // POST: MetodosPagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var metodosPago = await _context.MetodosPago.FindAsync(id);
            if (metodosPago != null)
            {
                _context.MetodosPago.Remove(metodosPago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MetodosPagoExists(Guid id)
        {
            return _context.MetodosPago.Any(e => e.MetodoId == id);
        }
    }
}

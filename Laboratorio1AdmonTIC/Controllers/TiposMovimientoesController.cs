﻿using System;
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
            return View(await _context.TiposMovimiento.ToListAsync());
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

            return View(tiposMovimiento);
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
                return RedirectToAction(nameof(Index));
            }
            return View(tiposMovimiento);
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
                return NotFound();
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tiposMovimiento);
        }

        // GET: TiposMovimientoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

            //return View(tiposMovimiento);
            tiposMovimiento.Inactivo = true;
            _context.Update(tiposMovimiento);
            await _context.SaveChangesAsync();

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

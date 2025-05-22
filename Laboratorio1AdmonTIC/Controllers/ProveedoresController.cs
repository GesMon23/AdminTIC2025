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
	public class ProveedoresController : Controller
    {
        private readonly ERPDbContext _context;

        public ProveedoresController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Proveedores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Proveedores.Where(c => !c.Inactivo).ToListAsync());
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedores = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedores == null)
            {
                return NotFound();
            }

            return PartialView("Details", proveedores);
        }


        


        // GET: Proveedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProveedorId,Nombre,Telefono,Direccion,Email")] Proveedores proveedores)
        {
            if (ModelState.IsValid)
            {
                proveedores.ProveedorId = Guid.NewGuid();
                _context.Add(proveedores);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proveedor registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Ocurrio un error al guardar.";
            return RedirectToAction(nameof(Create));
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedores = await _context.Proveedores.FindAsync(id);
            if (proveedores == null)
            {
                return NotFound();
            }
            return View(proveedores);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProveedorId,Nombre,Telefono,Direccion,Email")] Proveedores proveedores)
        {
            if (id != proveedores.ProveedorId)
            {
                TempData["Error"] = "Ocurrio un error al actualizar.";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedoresExists(proveedores.ProveedorId))
                    {
                        TempData["Error"] = "Ocurrio un error al actualizar.";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Categoría actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Modelo invalido (faltan datos).";
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // GET: Proveedores/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id no válido.";
                return RedirectToAction(nameof(Index));
            }

            var proveedores = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedores == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //return View(proveedores);
            proveedores.Inactivo = true;
            _context.Update(proveedores);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registro eliminado correctamente.";

            return RedirectToAction("Index");
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var proveedores = await _context.Proveedores.FindAsync(id);
            if (proveedores != null)
            {
                _context.Proveedores.Remove(proveedores);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedoresExists(Guid id)
        {
            return _context.Proveedores.Any(e => e.ProveedorId == id);
        }
    }
}

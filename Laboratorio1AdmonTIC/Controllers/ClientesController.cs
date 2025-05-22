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
	public class ClientesController : Controller
    {
        private readonly ERPDbContext _context;

        public ClientesController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.Where(c => !c.Inactivo).ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientes = await _context.Clientes
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (clientes == null)
            {
                return NotFound();
            }

            return PartialView("Details", clientes);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,Nombres,Apellidos,Telefono,Direccion,Email,CUI,NIT")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                clientes.ClienteId = Guid.NewGuid();
                _context.Add(clientes);
                TempData["Success"] = "Cliente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Ocurrio un error al guardar.";
            return RedirectToAction(nameof(Create));
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientes = await _context.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return NotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ClienteId,Nombres,Apellidos,Telefono,Direccion,Email,CUI,NIT")] Clientes clientes)
        {
            if (id != clientes.ClienteId)
            {
                TempData["Error"] = "Ocurrio un error no se encontro el id.";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientesExists(clientes.ClienteId))
                    {
                        TempData["Error"] = "Ocurrio un error al actualizar.";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Modelo invalido (faltan datos).";
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id no válido.";
                return RedirectToAction(nameof(Index));
            }

            var clientes = await _context.Clientes
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (clientes == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //return View(clientes);
            clientes.Inactivo = true;
            _context.Update(clientes);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Registro eliminado correctamente.";

            return RedirectToAction("Index");
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var clientes = await _context.Clientes.FindAsync(id);
            if (clientes != null)
            {
                _context.Clientes.Remove(clientes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientesExists(Guid id)
        {
            return _context.Clientes.Any(e => e.ClienteId == id);
        }
    }
}

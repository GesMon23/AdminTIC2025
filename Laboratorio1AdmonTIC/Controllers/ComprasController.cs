using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Laboratorio1AdmonTIC.Models;
using Laboratorio1AdmonTIC.ViewModels;

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
            var compras = await (
                from c in _context.Compras
                join p in _context.Proveedores on c.ProveedorId equals p.ProveedorId
                join e in _context.Empleados on c.EmpleadosId equals e.EmpleadosId
                where !c.Inactivo
                select new CompraViewModel
                {
                    CompraId = c.CompraId,
                    Proveedor = p.Nombre,
                    Empleado = e.Nombres + " " + e.Apellidos,
                    FechaCompra = c.FechaCompra,
                    Total = c.Total
                } ).ToListAsync();

            return View(compras);

        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compras == null)
            {
                return NotFound();
            }

            return View(compras);
        }

        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
            var compra = (from c in _context.Compras
                            join e in _context.Empleados on c.EmpleadosId equals e.EmpleadosId
                            join p in _context.Proveedores on c.ProveedorId equals p.ProveedorId
                            where !c.Inactivo && c.CompraId == id
                            select new CompraViewModel
                            {
                                CompraId = c.CompraId,
                                Proveedor = p.Nombre,
                                Empleado = e.Nombres + ' ' + e.Apellidos,
                                FechaCompra = c.FechaCompra,
                                Total = c.Total
                            }).FirstOrDefault();

            if (compra == null)
            {
                return NotFound();
            }

            return PartialView("Details", compra);
        }


        // GET: Compras/Create
        public IActionResult Create()
        {
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new {Id = e.EmpleadosId,NombreCompleto = e.Nombres + " " + e.Apellidos}),"Id","NombreCompleto");

            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
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
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, NombreCompleto = e.Nombres + " " + e.Apellidos }), "Id", "NombreCompleto");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
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
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, NombreCompleto = e.Nombres + " " + e.Apellidos }), "Id", "NombreCompleto");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
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
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, NombreCompleto = e.Nombres + " " + e.Apellidos }), "Id", "NombreCompleto");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
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

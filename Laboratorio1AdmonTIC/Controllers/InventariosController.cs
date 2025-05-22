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
            //return View(await _context.Inventario.Where(c => !c.Inactivo).ToListAsync());
            var movimientos = await (from i in _context.Inventario
                                     join p in _context.Productos on i.ProductoId equals p.ProductoId
                                     join tm in _context.TiposMovimiento on i.TipoMovimientoId equals tm.TipoMovimientoId
                                     join e in _context.Empleados on i.EmpleadosId equals e.EmpleadosId
                                     where !i.Inactivo
                                     select new MovimientoInventarioViewModel
                                     {
                                        MovimientoId = i.MovimientoId,
                                        ProductoNombre = p.Nombre,
                                        TipoMovimiento = tm.TipoMovimiento,
                                        Empleado = e.Nombres + " " + e.Apellidos,
                                        Cantidad = i.Cantidad,
                                        FechaCompra = i.FechaCompra
                                     }).ToListAsync();
            return View(movimientos);

        }

        // GET: Inventarios/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                //.Include(i => i.Empleados)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
            var inventario = (from i in _context.Inventario
                         join p in _context.Productos on i.ProductoId equals p.ProductoId
                         join tm in _context.TiposMovimiento on i.TipoMovimientoId equals tm.TipoMovimientoId
                         join e in _context.Empleados on i.EmpleadosId equals e.EmpleadosId
                         where !i.Inactivo && i.MovimientoId == id
                         select new MovimientoInventarioViewModel
                         {
                             MovimientoId = i.MovimientoId,
                             ProductoNombre = p.Nombre,
                             TipoMovimiento = tm.TipoMovimiento,
                             Empleado = e.Nombres + ' ' + e.Apellidos,
                             Cantidad = i.Cantidad,
                             FechaCompra = i.FechaCompra
                         }).FirstOrDefault();

            if (inventario == null)
            {
                return NotFound();
            }

            return PartialView("Details", inventario);
        }


        // GET: Inventarios/Create
        public IActionResult Create()
        {//producto, TipoMovimiento, empleado
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.TipoMovimiento = new SelectList(_context.TiposMovimiento.Where(p => !p.Inactivo), "TipoMovimientoId", "TipoMovimiento");
            ViewBag.Producto = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
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
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.TipoMovimiento = new SelectList(_context.TiposMovimiento.Where(p => !p.Inactivo), "TipoMovimientoId", "TipoMovimiento");
            ViewBag.Producto = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
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
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.TipoMovimiento = new SelectList(_context.TiposMovimiento.Where(p => !p.Inactivo), "TipoMovimientoId", "TipoMovimiento");
            ViewBag.Producto = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
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
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.TipoMovimiento = new SelectList(_context.TiposMovimiento.Where(p => !p.Inactivo), "TipoMovimientoId", "TipoMovimiento");
            ViewBag.Producto = new SelectList(_context.Productos.Where(p => !p.Inactivo), "ProductoId", "Nombre");
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
                //.Include(i => i.Empleados)
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

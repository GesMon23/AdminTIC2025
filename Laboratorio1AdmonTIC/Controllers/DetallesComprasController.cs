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
            //return View(await _context.DetallesCompras.Where(c => !c.Inactivo).ToListAsync());
            var dcompras = await (from dc in _context.DetallesCompras
                                  join p in _context.Productos on dc.ProductoId equals p.ProductoId
                                  where !dc.Inactivo
                                  select new DetallesComprasViewModel
                                  {
                                      DetalleCompraId = dc.DetalleCompraId,
                                      CompraId = dc.CompraId,
                                      Producto = p.Nombre,
                                      Cantidad = dc.Cantidad,
                                      PrecioUnitario = dc.PrecioUnitario,
                                      Total = dc.Total
                                  }).ToListAsync();
            return View(dcompras);
        }

        // GET: DetallesCompras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesCompras = (from dc in _context.DetallesCompras
                                   join p in _context.Productos on dc.ProductoId equals p.ProductoId
                                   where !dc.Inactivo && dc.DetalleCompraId == id
                                   select new DetallesComprasViewModel 
                                   {
                                       DetalleCompraId = dc.DetalleCompraId,
                                       CompraId = dc.CompraId,
                                       Producto = p.Nombre,
                                       Descripcion = p.Descripcion,
                                       Cantidad = dc.Cantidad,
                                       PrecioUnitario = dc.PrecioUnitario,
                                       Total = dc.Total
                                   }).FirstOrDefault();
            if (detallesCompras == null)
            {
                return NotFound();
            }

            return View(detallesCompras);
        }

        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
            var detallesCompras = (from dc in _context.DetallesCompras
                                   join p in _context.Productos on dc.ProductoId equals p.ProductoId
                                   where !dc.Inactivo && dc.DetalleCompraId == id
                                   select new DetallesComprasViewModel
                                   {
                                       DetalleCompraId = dc.DetalleCompraId,
                                       CompraId = dc.CompraId,
                                       Producto = p.Nombre,
                                       Descripcion = p.Descripcion,
                                       Cantidad = dc.Cantidad,
                                       PrecioUnitario = dc.PrecioUnitario,
                                       Total = dc.Total
                                   }).FirstOrDefault();

            if (detallesCompras == null)
            {
                return NotFound();
            }

            return PartialView("Details", detallesCompras);
        }

        // GET: DetallesCompras/Create
        public IActionResult Create()
        {
            ViewBag.Compras = new SelectList(_context.Compras.Where(c => !c.Inactivo), "CompraId", "CompraId");
            ViewBag.Producto = new SelectList(_context.Productos.Where(c => !c.Inactivo), "ProductoId", "Nombre");
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
            ViewBag.Compras = new SelectList(_context.Compras.Where(c => !c.Inactivo), "CompraId");
            ViewBag.Producto = new SelectList(_context.Productos.Where(c => !c.Inactivo), "ProductoId", "Nombre");
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
            ViewBag.Compras = new SelectList(_context.Compras.Where(c => !c.Inactivo), "CompraId", "CompraId");
            ViewBag.Producto = new SelectList(_context.Productos.Where(c => !c.Inactivo), "ProductoId", "Nombre");
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
            ViewBag.Compras = new SelectList(_context.Compras.Where(c => !c.Inactivo), "CompraId", "CompraId");
            ViewBag.Producto = new SelectList(_context.Productos.Where(c => !c.Inactivo), "ProductoId", "Nombre");
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

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
    public class VentasController : Controller
    {
        private readonly ERPDbContext _context;

        public VentasController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Ventas.Where(c => !c.Inactivo).ToListAsync());
            var ventas = await (from v in _context.Ventas
                                join c in _context.Clientes on v.ClienteId equals c.ClienteId
                                join e in _context.Empleados on v.EmpleadosId equals e.EmpleadosId
                                join mp in _context.MetodosPago on v.MetodoId equals mp.MetodoId
                                where !v.Inactivo
                                select new VentaViewModel
                                {
                                    VentasId = v.VentasId,
                                    Cliente = c.Nombres + " " + c.Apellidos,
                                    Empleado = e.Nombres + " " + e.Apellidos,
                                    TipoPago = mp.TipoPago,
                                    FechaVenta = v.FechaVenta,
                                    Total = v.Total
                                }
                            ).ToListAsync();

            return View(ventas);
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventas = await _context.Ventas
                .FirstOrDefaultAsync(m => m.VentasId == id);
            if (ventas == null)
            {
                return NotFound();
            }

            return View(ventas);
        }


        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
            var venta = (from ve in _context.Ventas
                         join cl in _context.Clientes on ve.ClienteId equals cl.ClienteId
                         join em in _context.Empleados on ve.EmpleadosId equals em.EmpleadosId
                         join mepa in _context.MetodosPago on ve.MetodoId equals mepa.MetodoId
                         where !ve.Inactivo && ve.VentasId == id
                         select new VentaViewModel
                          {
                            VentasId = ve.ClienteId,
                            Cliente = cl.Nombres + ' ' + cl.Apellidos,
                            Empleado = em.Nombres + ' ' + em.Apellidos,
                            TipoPago = mepa.TipoPago,
                            FechaVenta = ve.FechaVenta,
                            Total = ve.Total
                          }).FirstOrDefault();

            if (venta == null)
            {
                return NotFound();
            }

            return PartialView("Details", venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            return View();
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VentasId,ClienteId,EmpleadosId,MetodoId,FechaVenta,Total")] Ventas ventas)
        {
            if (ModelState.IsValid)
            {
                ventas.VentasId = Guid.NewGuid();
                _context.Add(ventas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            return View(ventas);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventas = await _context.Ventas.FindAsync(id);
            if (ventas == null)
            {
                return NotFound();
            }
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            return View(ventas);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("VentasId,ClienteId,EmpleadosId,MetodoId,FechaVenta,Total")] Ventas ventas)
        {
            if (id != ventas.VentasId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ventas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentasExists(ventas.VentasId))
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
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            return View(ventas);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventas = await _context.Ventas
                .FirstOrDefaultAsync(m => m.VentasId == id);
            if (ventas == null)
            {
                return NotFound();
            }

            //return View(ventas);
            ventas.Inactivo = true;
            _context.Update(ventas);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ventas = await _context.Ventas.FindAsync(id);
            if (ventas != null)
            {
                _context.Ventas.Remove(ventas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentasExists(Guid id)
        {
            return _context.Ventas.Any(e => e.VentasId == id);
        }
    }
}

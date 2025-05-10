using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Laboratorio1AdmonTIC.Models;
using Laboratorio1AdmonTIC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Laboratorio1AdmonTIC.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly ERPDbContext _context;

        public EmpleadosController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Empleados.Where(c => !c.Inactivo).ToListAsync());
            var empleados = await (from empl in _context.Empleados
                                   join usuarios in _context.Users on empl.UserId equals usuarios.Id
                                   where !empl.Inactivo
                                   select new EmpleadosViewModels
                                   {
                                       EmpleadosId = empl.EmpleadosId,
                                       Nombres = empl.Nombres,
                                       Apellidos = empl.Apellidos,
                                       Cargo = empl.Cargo,
                                       Telefono = empl.Telefono,
                                       Email = empl.Email,
                                       Salario = empl.Salario,
                                       Usuario = usuarios.UserName
                                   }).ToListAsync();
            return View(empleados);
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleados = await _context.Empleados
                .FirstOrDefaultAsync(m => m.EmpleadosId == id);
            if (empleados == null)
            {
                return NotFound();
            }

            return View(empleados);
        }

        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
            var empleados = (from empl in _context.Empleados
                                   join usuarios in _context.Users on empl.UserId equals usuarios.Id
                                   where !empl.Inactivo && empl.EmpleadosId == id
                                   select new EmpleadosViewModels
                                   {
                                       EmpleadosId = empl.EmpleadosId,
                                       Nombres = empl.Nombres,
                                       Apellidos = empl.Apellidos,
                                       Cargo = empl.Cargo,
                                       Telefono = empl.Telefono,
                                       Email = empl.Email,
                                       Salario = empl.Salario,
                                       Usuario = usuarios.UserName
                                   }).FirstOrDefault();

            if (empleados == null)
            {
                Console.WriteLine("Empleado no encontrado.");
                return NotFound();
            }

            //Console.WriteLine("Empleado encontrado, renderizando vista parcial.");

            return PartialView("Details", empleados);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "UserName");
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpleadosId,Nombres,Apellidos,Cargo,Telefono,Email,Salario,UserId")] Empleados empleados)
        {
            if (_context.Empleados.Any(e => e.UserId == empleados.UserId))
            {
                TempData["ErrorEmpleado"] = "Este usuario ya tiene un empleado asignado.";
                return RedirectToAction(nameof(Create));
            }

            if (ModelState.IsValid)
            {
                empleados.EmpleadosId = Guid.NewGuid();
                _context.Add(empleados);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "UserName", empleados.UserId);
            return View(empleados);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleados = await _context.Empleados.FindAsync(id);
            if (empleados == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "UserName", empleados.UserId);
            return View(empleados);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EmpleadosId,Nombres,Apellidos,Cargo,Telefono,Email,Salario,UserId")] Empleados empleados)
        {
            if (_context.Empleados.Any(e => e.UserId == empleados.UserId && e.EmpleadosId != empleados.EmpleadosId))
            {
                TempData["ErrorEmpleado"] = "Este usuario ya tiene un empleado asignado.";
                return RedirectToAction(nameof(Edit));
            }


            if (id != empleados.EmpleadosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleados);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadosExists(empleados.EmpleadosId))
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
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "UserName", empleados.UserId);
            return View(empleados);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleados = await _context.Empleados
                .FirstOrDefaultAsync(m => m.EmpleadosId == id);
            if (empleados == null)
            {
                return NotFound();
            }

            //return View(empleados);
            empleados.Inactivo = true;
            _context.Update(empleados);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var empleados = await _context.Empleados.FindAsync(id);
            if (empleados != null)
            {
                _context.Empleados.Remove(empleados);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadosExists(Guid id)
        {
            return _context.Empleados.Any(e => e.EmpleadosId == id);
        }
    }
}

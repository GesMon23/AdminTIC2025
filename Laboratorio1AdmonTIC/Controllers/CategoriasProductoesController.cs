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
    public class CategoriasProductoesController : Controller
    {
        private readonly ERPDbContext _context;

        public CategoriasProductoesController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: CategoriasProductoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriasProducto.Where(c => !c.Inactivo).ToListAsync());
        }

        // GET: CategoriasProductoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriasProducto = await _context.CategoriasProducto
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriasProducto == null)
            {
                return NotFound();
            }

            return View(categoriasProducto);
        }

        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriasProducto = await _context.CategoriasProducto
                .FirstOrDefaultAsync(m => m.CategoriaId == id);

            if (categoriasProducto == null)
            {
                return NotFound();
            }

            return PartialView("Details", categoriasProducto);
        }

        // GET: CategoriasProductoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriasProductoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoriaId,Nombre")] CategoriasProducto categoriasProducto)
        {
            if (ModelState.IsValid)
            {
                categoriasProducto.CategoriaId = Guid.NewGuid();
                _context.Add(categoriasProducto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Categoría registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
			TempData["Error"] = "Ocurrio un error al guardar.";
			return RedirectToAction(nameof(Create));
		}

        // GET: CategoriasProductoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriasProducto = await _context.CategoriasProducto.FindAsync(id);
            if (categoriasProducto == null)
            {
                return NotFound();
            }
            return View(categoriasProducto);
        }

        // POST: CategoriasProductoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CategoriaId,Nombre")] CategoriasProducto categoriasProducto)
        {
            if (id != categoriasProducto.CategoriaId)
            {
				TempData["Error"] = "Ocurrio un error al actualizar.";
                return RedirectToAction(nameof(Edit), new { id = id });
			}

			if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriasProducto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriasProductoExists(categoriasProducto.CategoriaId))
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

		// GET: CategoriasProductoes/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
				TempData["Error"] = "Id no válido.";
				return RedirectToAction(nameof(Index));
            }

            var categoriasProducto = await _context.CategoriasProducto
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriasProducto == null)
            {
				TempData["Error"] = "Registro no encontrado.";
				return RedirectToAction(nameof(Index));
			}

            //return View(categoriasProducto);

            categoriasProducto.Inactivo = true;
            _context.Update(categoriasProducto);
            await _context.SaveChangesAsync();

			TempData["Success"] = "Registro eliminado correctamente.";

			return RedirectToAction("Index");
        }

        // POST: CategoriasProductoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var categoriasProducto = await _context.CategoriasProducto.FindAsync(id);
            if (categoriasProducto != null)
            {
                _context.CategoriasProducto.Remove(categoriasProducto);
                TempData["Success"] = "Registro eliminado Correctamente.";
            }
            else {
				TempData["Error"] = "Ocurrio un error al eliminar el registro.";
				return RedirectToAction(nameof(Index));
			}

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriasProductoExists(Guid id)
        {
            return _context.CategoriasProducto.Any(e => e.CategoriaId == id);
        }
    }
}

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
	public class ProductosController : Controller
	{
		private readonly ERPDbContext _context;

		public ProductosController(ERPDbContext context)
		{
			_context = context;
		}

		// GET: Productos
		public async Task<IActionResult> Index()
		{
			//var productos = _context.Productos
			//                       .GroupJoin(_context.Proveedores, p => p.ProveedorId, prov => prov.ProveedorId, (p, prov) => new { p, prov })
			//                       .SelectMany(x => x.prov.DefaultIfEmpty(), (x, prov) => new { x.p, Proveedor = prov })
			//                       .GroupJoin(_context.CategoriasProducto, x => x.p.CategoriaId, c => c.CategoriaId, (x, c) => new { x.p, x.Proveedor, c })
			//                       .SelectMany(x => x.c.DefaultIfEmpty(), (x, c) => new { x.p, x.Proveedor, Categoria = c })
			//                       .Where(x => !x.p.Inactivo);

			//return View(productos);
			//return View(await _context.Productos.Where(c => !c.Inactivo).ToListAsync());
			var productos = await (from produc in _context.Productos
							join catpro in _context.CategoriasProducto on produc.CategoriaId equals catpro.CategoriaId
							join prov in _context.Proveedores on produc.ProveedorId equals prov.ProveedorId
							where !produc.Inactivo
							select new ProductoViewModel
                            {
                                Proveedor = prov.Nombre,
								Categoria = catpro.Nombre,
								Id = produc.ProductoId,
								Nombre = produc.Nombre,
                                Descripcion = produc.Descripcion,
                                PrecioCompra = produc.PrecioCompra,
                                PrecioVenta = produc.PrecioVenta,
                                Stock = produc.Stock,
								StockMinimo = produc.StockMinimo,
                                UnidadMedida = produc.UnidadMedida
                            }).ToListAsync();
			return View(productos);
		}

		// GET: Productos/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

            //var productos = await _context.Productos
            //	.FirstOrDefaultAsync(m => m.ProductoId == id);
            var productos = (from produc in _context.Productos
                                   join catpro in _context.CategoriasProducto on produc.CategoriaId equals catpro.CategoriaId
                                   join prov in _context.Proveedores on produc.ProveedorId equals prov.ProveedorId
                                   where !produc.Inactivo && produc.ProductoId == id
                                   select new ProductoViewModel
                                   {
                                       Proveedor = prov.Nombre,
                                       Categoria = catpro.Nombre,
                                       Id = produc.ProductoId,
                                       Nombre = produc.Nombre,
                                       Descripcion = produc.Descripcion,
                                       PrecioCompra = produc.PrecioCompra,
                                       PrecioVenta = produc.PrecioVenta,
                                       Stock = produc.Stock,
                                       StockMinimo = produc.StockMinimo,
                                       UnidadMedida = produc.UnidadMedida
                                   }).FirstOrDefault();
            if (productos == null)
			{
				return NotFound();
			}
            
            return View(productos);
		}
        public async Task<IActionResult> DetailsPartial(Guid id)
        {
			//Console.WriteLine($"Producto con ID {id} no encontrado");
            var producto = (from produc in _context.Productos
                             join catpro in _context.CategoriasProducto on produc.CategoriaId equals catpro.CategoriaId
                             join prov in _context.Proveedores on produc.ProveedorId equals prov.ProveedorId
                             where !produc.Inactivo && produc.ProductoId == id
                             select new ProductoViewModel
                             {
                                 Proveedor = prov.Nombre,
                                 Categoria = catpro.Nombre,
                                 Id = produc.ProductoId,
                                 Nombre = produc.Nombre,
                                 Descripcion = produc.Descripcion,
                                 PrecioCompra = produc.PrecioCompra,
                                 PrecioVenta = produc.PrecioVenta,
                                 Stock = produc.Stock,
                                 StockMinimo = produc.StockMinimo,
                                 UnidadMedida = produc.UnidadMedida
                             }).FirstOrDefault();

			if (producto == null) { 
                return NotFound();
            }

            return PartialView("Details", producto);
        }


        // GET: Productos/Create webmdolboeba7 naygad
        public IActionResult Create()
		{
            ViewBag.Categorias = new SelectList(_context.CategoriasProducto.Where(c => !c.Inactivo), "CategoriaId", "Nombre");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
            return View();
		}

		// POST: Productos/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ProductoId,CategoriaId,ProveedorId,Nombre,Descripcion,PrecioCompra,PrecioVenta,Stock,StockMinimo,UnidadMedida")] Productos productos)
		{
			if (ModelState.IsValid)
			{
                productos.ProductoId = Guid.NewGuid();
				_context.Add(productos);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Producto registrado correctamente.";
				return RedirectToAction(nameof(Index));
			}

            ViewBag.Categorias = new SelectList(_context.CategoriasProducto.Where(c => !c.Inactivo), "CategoriaId", "Nombre");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
			TempData["Error"] = "Ocurrio un error al guardar.";
			return RedirectToAction(nameof(Create));
		}

		// GET: Productos/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var productos = await _context.Productos.FindAsync(id);
			if (productos == null)
			{
				return NotFound();
			}
            ViewBag.Categorias = new SelectList(_context.CategoriasProducto.Where(c => !c.Inactivo), "CategoriaId", "Nombre");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");

            return View(productos);
		}

		// POST: Productos/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("ProductoId,CategoriaId,ProveedorId,Nombre,Descripcion,PrecioCompra,PrecioVenta,Stock,StockMinimo,UnidadMedida")] Productos productos)
		{
			if (id != productos.ProductoId)
			{
				TempData["Error"] = "Ocurrio un error al actualizar.";
				return RedirectToAction(nameof(Edit), new { id = id });
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(productos);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductosExists(productos.ProductoId))
					{
						TempData["Error"] = "Ocurrio un error al actualizar.";
						return RedirectToAction(nameof(Edit), new { id = id });
					}
					else
					{
						throw;
					}
				}
				TempData["Success"] = "Producto actualizado correctamente.";
				return RedirectToAction(nameof(Index));
			}
			TempData["Error"] = "Modelo invalido (faltan datos).";
			return RedirectToAction(nameof(Edit), new { id = id });
		}

		// GET: Productos/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null)
			{
				TempData["Error"] = "Id no válido.";
				return RedirectToAction(nameof(Index));
			}

			var productos = await _context.Productos
				.FirstOrDefaultAsync(m => m.ProductoId == id);
			if (productos == null)
			{
				TempData["Error"] = "Registro no encontrado.";
				return RedirectToAction(nameof(Index));
			}

			//return View(productos);
			productos.Inactivo = true;
			_context.Update(productos);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Registro eliminado correctamente.";

			return RedirectToAction("Index");
		}

		// POST: Productos/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var productos = await _context.Productos.FindAsync(id);
			if (productos != null)
			{
				_context.Productos.Remove(productos);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProductosExists(Guid id)
		{
			return _context.Productos.Any(e => e.ProductoId == id);
		}
	}
}

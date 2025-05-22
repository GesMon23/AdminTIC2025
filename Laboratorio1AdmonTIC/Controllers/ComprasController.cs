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
            // En tu controlador

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
                            where c.CompraId == id
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


            var detalles = (from dv in _context.DetallesCompras
                            join pr in _context.Productos on dv.ProductoId equals pr.ProductoId
                            where dv.CompraId == id
                            select new DetallesComprasViewModel
                            {
                                productoId = pr.ProductoId,
                                Producto = pr.Nombre,
                                Cantidad = dv.Cantidad,
                                PrecioUnitario = dv.PrecioUnitario,
                                Total = dv.Total
                            }).ToList();

            var viewModel = new DetallesCviewModel
            {
                Compra = compra,
                Detalles = detalles
            };


            return PartialView("Details", viewModel);
        }


        // GET: Compras/Create
        public IActionResult Create()
        {
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new {Id = e.EmpleadosId,NombreCompleto = e.Nombres + " " + e.Apellidos}),"Id","NombreCompleto");

            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");


            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            //ViewBag.Producto = new SelectList(listaProductos, "ProductoId", "Nombre");
            return View();
        }

        


        // POST: Compras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetallesInsertViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Compra.CompraId = Guid.NewGuid();
                model.Compra.FechaCompra = DateTime.Now;
                _context.Add(model.Compra);
                //TipoMovimientoId Ingreso --> 9E1EA92E-D21A-4E54-9A44-40947ECFFB5A


                Guid idCompra = model.Compra.CompraId;
                Guid tipoMovimientoIngresoId = Guid.Parse("9E1EA92E-D21A-4E54-9A44-40947ECFFB5A");

                foreach (var detalle in model.DetallesCompras)
                {
                    //Console.WriteLine($"ProductoId: {detalle.ProductoId}, Cantidad: {detalle.Cantidad}, Precio: {detalle.PrecioUnitario}, Subtotal: {detalle.Total}");
                    detalle.DetalleCompraId = Guid.NewGuid();
                    detalle.CompraId = idCompra;
                    _context.DetallesCompras.Add(detalle);

                    var invent = new Inventario
                    {
                        MovimientoId = Guid.NewGuid(),
                        ProductoId = detalle.ProductoId,
                        TipoMovimientoId = tipoMovimientoIngresoId,
                        EmpleadosId = model.Compra.EmpleadosId,
                        Cantidad = detalle.Cantidad,
                        FechaCompra = DateTime.Now,
                        CompraId = idCompra,
                        Inactivo = false
                    };
                    _context.Inventario.Add(invent);

                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                        _context.Productos.Update(producto);
                    }
                    else {
                        //Aqui puedo colocar una alerta
                        TempData["Warning"] = $"Producto con ID {detalle.ProductoId} no encontrado para actualizar stock.";
                    }
                }
                await _context.SaveChangesAsync();
                TempData["Success"] = "Registros agregados correctamente.";
                return RedirectToAction(nameof(Index));

            }
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, NombreCompleto = e.Nombres + " " + e.Apellidos }), "Id", "NombreCompleto");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            TempData["Error"] = "Modelo invalido (faltaron datos).";
            return RedirectToAction(nameof(Create));
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compraPrincipal = await _context.Compras.FindAsync(id);

            var detalles = await (from dc in _context.DetallesCompras
                                  join p in _context.Productos on dc.ProductoId equals p.ProductoId
                                  where dc.CompraId == id
                                  select new DetallesComprasViewModel
                                  {
                                      DetalleCompraId = dc.DetalleCompraId,
                                      Producto = p.Nombre,
                                      productoId = dc.ProductoId,
                                      Cantidad = dc.Cantidad,
                                      PrecioUnitario = dc.PrecioUnitario,
                                      Total = dc.Total
                                  }).ToListAsync();

            if (compraPrincipal == null )
            {
                return NotFound();
            }

            var vm = new CompraConDetallesViewModel
            {
                compraId = compraPrincipal.CompraId,
                Compra = new Compras
                {
                    ProveedorId = compraPrincipal.ProveedorId,
                    EmpleadosId = compraPrincipal.EmpleadosId,
                    FechaCompra = compraPrincipal.FechaCompra,
                    Total = compraPrincipal.Total
                },
                Detalles= detalles
            };

            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, NombreCompleto = e.Nombres + " " + e.Apellidos }), "Id", "NombreCompleto");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            return View(vm);
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
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            return View(compras);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id no válido.";
                return RedirectToAction(nameof(Index));
            }

            var compras = await _context.Compras
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compras == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //return View(compras);
            compras.Inactivo = true;

            var detallesc = await _context.DetallesCompras
                .Where(dc => dc.CompraId == id)
                .ToListAsync();

            List<Productos> productosAActualizar = new List<Productos>();

            foreach (var detalle in detallesc)
            {
                detalle.Inactivo = true;

                var productos = await _context.Productos
                    .FirstOrDefaultAsync(p => p.ProductoId == detalle.ProductoId);
                if (productos != null)
                {
                    productos.Stock -= detalle.Cantidad;
                    productosAActualizar.Add(productos);
                }
                else{
                    TempData["Warning"] = $"Producto con ID {detalle.ProductoId} no encontrado al eliminar.";
                }
            }

            var inventari = await _context.Inventario
                    .Where(i => i.CompraId == id)
                    .ToListAsync();
            foreach (var invent in inventari)
            {
                    invent.Inactivo = true;
            }

            _context.Update(compras);
            _context.UpdateRange(detallesc);
            _context.UpdateRange(inventari);
            _context.UpdateRange(productosAActualizar);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Registro eliminado correctamente.";
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


        [HttpGet]
        public IActionResult ObtenerPrecio(Guid idProducto)
        {
            var producto = _context.Productos
                .FirstOrDefault(p => p.ProductoId == idProducto);

            if (producto == null)
            {
                return NotFound();
            }

            return Json(new { precio = producto.PrecioCompra });
        }

        public async Task<IActionResult> Anulados()
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");
           
            var compraA = await (
                from c in _context.Compras
                join p in _context.Proveedores on c.ProveedorId equals p.ProveedorId
                join e in _context.Empleados on c.EmpleadosId equals e.EmpleadosId
                where c.Inactivo
                select new CompraViewModel
                {
                    CompraId = c.CompraId,
                    Proveedor = p.Nombre,
                    Empleado = e.Nombres + " " + e.Apellidos,
                    FechaCompra = c.FechaCompra,
                    Total = c.Total
                }).ToListAsync();

            if (!compraA.Any())
            {
                return NotFound();
            }

            return PartialView("_TablaAnulados", compraA);
        }

        public async Task<IActionResult> Reactivar(Guid? id)
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
            compras.Inactivo = false;

            var detallesc = await _context.DetallesCompras
                .Where(dc => dc.CompraId == id)
                .ToListAsync();

            List<Productos> productosAActualizar = new List<Productos>();

            foreach (var detalle in detallesc)
            {
                detalle.Inactivo = false;

                var productos = await _context.Productos
                    .FirstOrDefaultAsync(p => p.ProductoId == detalle.ProductoId);
                if (productos != null)
                {
                    productos.Stock += detalle.Cantidad;
                    productosAActualizar.Add(productos);
                }
            }

            var inventari = await _context.Inventario
                    .Where(i => i.CompraId == id)
                    .ToListAsync();
            foreach (var invent in inventari)
            {
                invent.Inactivo = false;
            }

            _context.Update(compras);
            _context.UpdateRange(detallesc);
            _context.UpdateRange(inventari);
            _context.UpdateRange(productosAActualizar);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDos(CompraConDetallesViewModel model)
        {
            Console.WriteLine("TOTAL:-------->"+model.Compra.Total);
            Console.WriteLine("SUBTOTAL:-------->"+model.Subtotal);
            if (ModelState.IsValid)
            {
                model.Compra.CompraId = Guid.NewGuid();
                model.Compra.FechaCompra = DateTime.Now;
                _context.Add(model.Compra);
                //TipoMovimientoId Ingreso --> 9E1EA92E-D21A-4E54-9A44-40947ECFFB5A


                Guid idCompra = model.Compra.CompraId;
                Guid tipoMovimientoIngresoId = Guid.Parse("9E1EA92E-D21A-4E54-9A44-40947ECFFB5A");

                foreach (var detalle in model.Detalles)
                {
                    var detalleCompra = new DetallesCompras
                    {
                        DetalleCompraId = Guid.NewGuid(),
                        CompraId = idCompra,
                        ProductoId = detalle.productoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Total = detalle.Total
                    };

                    _context.DetallesCompras.Add(detalleCompra);

                    var invent = new Inventario
                    {
                        MovimientoId = Guid.NewGuid(),
                        ProductoId = detalle.productoId,
                        TipoMovimientoId = tipoMovimientoIngresoId,
                        EmpleadosId = model.Compra.EmpleadosId,
                        Cantidad = detalle.Cantidad,
                        FechaCompra = DateTime.Now,
                        CompraId = idCompra,
                        Inactivo = false
                    };
                    _context.Inventario.Add(invent);

                    var producto = await _context.Productos.FindAsync(detalle.productoId);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                        _context.Productos.Update(producto);
                    }
                    else
                    {
                        TempData["Warning"] = $"Producto con ID {detalle.productoId} no encontrado para actualizar stock.";
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Compra y detalles agregados correctamente.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Campo: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }
                TempData["Error"] = "Datos inválidos al intentar crear la compra.";
                return RedirectToAction("Edit", new { id = model.compraId });
            }
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, NombreCompleto = e.Nombres + " " + e.Apellidos }), "Id", "NombreCompleto");
            ViewBag.Proveedores = new SelectList(_context.Proveedores.Where(p => !p.Inactivo), "ProveedorId", "Nombre");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            TempData["Error"] = "Ocurrió un error inesperado.";
            return RedirectToAction(nameof(Create));
        }






    }



}

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
            var venta = (from ve in _context.Ventas
                         join cl in _context.Clientes on ve.ClienteId equals cl.ClienteId
                         join em in _context.Empleados on ve.EmpleadosId equals em.EmpleadosId
                         join mepa in _context.MetodosPago on ve.MetodoId equals mepa.MetodoId
                         where ve.VentasId == id
                         select new VentaViewModel
                         {
                             VentasId = ve.VentasId,
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

            var detalles = (from dv in _context.DetallesVenta
                            join pr in _context.Productos on dv.ProductoId equals pr.ProductoId
                            where dv.VentasId == id
                            select new DetallesVentasViewModel
                            {
                                productoId = pr.ProductoId,
                                Producto = pr.Nombre,
                                Cantidad = dv.Cantidad,
                                PrecioUnitario = dv.PrecioUnitario,
                                Total = dv.Total
                            }).ToList();

            var viewModel = new DetallesVViewModel
            {
                Venta = venta,
                Detalles = detalles
            };

            return PartialView("Details", viewModel);
        }


        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            return View();
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetallesInsertVentasViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Ventas.VentasId = Guid.NewGuid();
                model.Ventas.FechaVenta = DateTime.Now;
                _context.Add(model.Ventas);


                await _context.SaveChangesAsync();

                Guid idVenta = model.Ventas.VentasId;
                Guid tipoMovimientoIngresoId = Guid.Parse("2519078B-66EF-44A8-9D25-5D0D3FF95B15");


                // Validar primero el stock de todos los productos
                foreach (var detalle in model.DetallesVentas)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                    {
                        TempData["Error"] = $"Producto con ID {detalle.ProductoId} no encontrado.";
                        //ModelState.AddModelError("", $"Producto con ID {detalle.ProductoId} no encontrado.");
                        return RedirectToAction(nameof(Create));
                    }

                    if (detalle.Cantidad > producto.Stock)
                    {
                        //ModelState.AddModelError("", $"No hay suficiente stock para el producto '{producto.Nombre}'. Stock disponible: {producto.Stock}, solicitado: {detalle.Cantidad}.");
                        TempData["Warning"] = $"No hay suficiente stock para el producto '{producto.Nombre}'. Stock disponible: {producto.Stock}, solicitado: {detalle.Cantidad}.";
                        // Cargar ViewBags antes de retornar
                        ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
                        ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
                        ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
                        ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
                        return RedirectToAction(nameof(Create));
                    }
                }


                //model.Ventas.VentasId = Guid.NewGuid();
                //_context.Add(model.Ventas);
                //await _context.SaveChangesAsync();


                

                foreach (var detalle in model.DetallesVentas)
                {
                    //Console.WriteLine($"ProductoId: {detalle.ProductoId}, Cantidad: {detalle.Cantidad}, Precio: {detalle.PrecioUnitario}, Subtotal: {detalle.Total}");

                    detalle.DetalleVentaId = Guid.NewGuid();
                    detalle.VentasId = idVenta;
                    _context.DetallesVenta.Add(detalle);

                    var invent = new Inventario
                    {
                        MovimientoId = Guid.NewGuid(),
                        ProductoId = detalle.ProductoId,
                        TipoMovimientoId = tipoMovimientoIngresoId,
                        EmpleadosId = model.Ventas.EmpleadosId,
                        Cantidad = detalle.Cantidad,
                        FechaCompra = DateTime.Now,
                        VentaId = idVenta,
                        Inactivo = false
                    };
                    _context.Inventario.Add(invent);

                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;
                        _context.Productos.Update(producto);
                    }
                    else
                    {
                        TempData["Error"] = $"Producto con ID {detalle.ProductoId} no encontrado para actualizar stock.";
                    }
                }
                await _context.SaveChangesAsync();
                TempData["Success"] = "Venta realizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            TempData["Error"] = "Algo fallo, vuelve a ingresar los datos.";
            return RedirectToAction(nameof(Create));
        }


        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventaPrincipal = await _context.Ventas.FindAsync(id);

            var detalles = await (from dc in _context.DetallesVenta
                                  join p in _context.Productos on dc.ProductoId equals p.ProductoId
                                  where dc.VentasId == id
                                  select new DetallesVentasViewModel
                                  {
                                      DetalleVentaId = dc.DetalleVentaId,
                                      Producto = p.Nombre,
                                      productoId = dc.ProductoId,
                                      Cantidad = dc.Cantidad,
                                      PrecioUnitario = dc.PrecioUnitario,
                                      Total = dc.Total
                                  }).ToListAsync();

            if (ventaPrincipal == null)
            {
                return NotFound();
            }

            var vm = new VentaConDetallesViewModel
            {
                ventaId = ventaPrincipal.VentasId,
                Ventas = new Ventas
                {
                    ClienteId = ventaPrincipal.ClienteId,
                    EmpleadosId = ventaPrincipal.EmpleadosId,
                    FechaVenta = ventaPrincipal.FechaVenta,
                    Total = ventaPrincipal.Total,
                    MetodoId = ventaPrincipal.MetodoId
                    
                },
                Detalles = detalles
            };

            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            return View(vm);
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
                TempData["Error"] = "Id no válido.";
                return RedirectToAction(nameof(Index));
            }

            var ventas = await _context.Ventas
                .FirstOrDefaultAsync(m => m.VentasId == id);
            if (ventas == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //return View(compras);
            ventas.Inactivo = true;

            var detallesc = await _context.DetallesVenta
                .Where(dc => dc.VentasId == id)
                .ToListAsync();

            List<Productos> productosAActualizar = new List<Productos>();

            foreach (var detalle in detallesc)
            {
                detalle.Inactivo = true;

                var productos = await _context.Productos
                    .FirstOrDefaultAsync(p => p.ProductoId == detalle.ProductoId);
                if (productos != null)
                {
                    productos.Stock += detalle.Cantidad;
                    productosAActualizar.Add(productos);
                }
                else {
                    TempData["Warning"] = $"Producto con ID {detalle.ProductoId} no encontrado al eliminar.";
                }
            }

            var inventari = await _context.Inventario
                    .Where(i => i.VentaId == id)
                    .ToListAsync();
            foreach (var invent in inventari)
            {
                invent.Inactivo = true;
            }

            _context.Update(ventas);
            _context.UpdateRange(detallesc);
            _context.UpdateRange(inventari);
            _context.UpdateRange(productosAActualizar);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Registro eliminado correctamente.";
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

        [HttpGet]
        public IActionResult ObtenerPrecio(Guid idProducto)
        {
            var producto = _context.Productos
                .FirstOrDefault(p => p.ProductoId == idProducto);

            if (producto == null)
            {
                return NotFound();
            }

            return Json(new { precio = producto.PrecioVenta });
        }

        [HttpGet]
        public IActionResult ObtenerStock(Guid idProducto)
        {
            var producto = _context.Productos
                .FirstOrDefault(p => p.ProductoId == idProducto);

            if (producto == null)
            {
                return NotFound();
            }

            return Json(new { stock = producto.Stock });
        }


        public async Task<IActionResult> Anulados()
        {
            //Console.WriteLine($"Producto con ID {id} no encontrado");

            var ventaA = await (
                from c in _context.Ventas
                join p in _context.Clientes on c.ClienteId equals p.ClienteId
                join e in _context.Empleados on c.EmpleadosId equals e.EmpleadosId
                join mp in _context.MetodosPago on c.MetodoId equals mp.MetodoId
                where c.Inactivo
                select new VentaViewModel
                {
                    VentasId = c.VentasId,
                    Cliente = p.Nombres,
                    Empleado = e.Nombres + " " + e.Apellidos,
                    FechaVenta = c.FechaVenta,
                    Total = c.Total,
                    TipoPago = mp.TipoPago
                }).ToListAsync();

            if (!ventaA.Any())
            {
                return NotFound();
            }

            return PartialView("_TablaAnulados", ventaA);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDos(VentaConDetallesViewModel model)
        {
            Console.WriteLine("TOTAL:-------->" + model.Ventas.Total);
            Console.WriteLine("SUBTOTAL:-------->" + model.Subtotal);
            if (ModelState.IsValid)
            {
                model.Ventas.VentasId = Guid.NewGuid();
                model.Ventas.FechaVenta = DateTime.Now;
                _context.Add(model.Ventas);
                //TipoMovimientoId Ingreso --> 9E1EA92E-D21A-4E54-9A44-40947ECFFB5A

                await _context.SaveChangesAsync();

                Guid idVenta = model.Ventas.VentasId;
                Guid tipoMovimientoIngresoId = Guid.Parse("2519078B-66EF-44A8-9D25-5D0D3FF95B15");

                foreach (var detalle in model.Detalles)
                {
                    var detalleVenta= new DetallesVenta
                    {
                        DetalleVentaId = Guid.NewGuid(),
                        VentasId = idVenta,
                        ProductoId = detalle.productoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Total = detalle.Total
                    };

                    _context.DetallesVenta.Add(detalleVenta);

                    var invent = new Inventario
                    {
                        MovimientoId = Guid.NewGuid(),
                        ProductoId = detalle.productoId,
                        TipoMovimientoId = tipoMovimientoIngresoId,
                        EmpleadosId = model.Ventas.EmpleadosId,
                        Cantidad = detalle.Cantidad,
                        FechaCompra = DateTime.Now,
                        VentaId = idVenta,
                        Inactivo = false
                    };
                    _context.Inventario.Add(invent);

                    var producto = await _context.Productos.FindAsync(detalle.productoId);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;
                        _context.Productos.Update(producto);
                    }
                    else
                    {
                        TempData["Warning"] = $"Producto con ID {detalle.productoId} no encontrado para actualizar stock.";
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Venta y detalles agregados correctamente.";
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

                TempData["Error"] = "Datos inválidos al intentar crear la venta.";
                return RedirectToAction("Edit", new { id = model.ventaId });
            }
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(e => !e.Inactivo).Select(e => new { Id = e.ClienteId, Cliente = e.Nombres + " " + e.Apellidos }), "Id", "Cliente");
            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => !e.Inactivo).Select(e => new { Id = e.EmpleadosId, Empleado = e.Nombres + " " + e.Apellidos }), "Id", "Empleado");
            ViewBag.MetodoPago = new SelectList(_context.MetodosPago.Where(p => !p.Inactivo), "MetodoId", "TipoPago");
            ViewBag.Producto = _context.Productos.Where(p => !p.Inactivo).ToList();
            TempData["Error"] = "Ocurrió un error inesperado.";
            return RedirectToAction(nameof(Create));
        }


    }
}

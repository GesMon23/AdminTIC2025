using Laboratorio1AdmonTIC.Models;
namespace Laboratorio1AdmonTIC.ViewModels
{
    public class DetallesInsertViewModel
    {
        public Compras Compra { get; set; }
        public List<DetallesCompras> DetallesCompras { get; set; }

        //public string nombreProducto { get; set; }


        // Propiedades auxiliares para mostrar en la vista
        //public Guid CompraId => Compra?.CompraId ?? Guid.Empty;
        //public Guid Proveedor => Compra?.ProveedorId ?? Guid.Empty;
        //public Guid Empleado => Compra?.EmpleadosId ?? Guid.Empty;
        //public DateTime FechaCompra => Compra?.FechaCompra ?? DateTime.MinValue;
        //public double Total => Compra?.Total ?? 0;
    }


}

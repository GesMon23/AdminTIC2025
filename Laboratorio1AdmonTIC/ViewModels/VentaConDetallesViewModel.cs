using Laboratorio1AdmonTIC.Models;

namespace Laboratorio1AdmonTIC.ViewModels
{
    public class VentaConDetallesViewModel
    {
        public Guid ventaId { get; set; }

        public Ventas Ventas { get; set; }
        public Guid ClienteId { get; set; }
        public Guid EmpleadoId { get; set; }
        public DateTime FechaCompra { get; set; }
        public double Subtotal { get; set; }
        public List<DetallesVentasViewModel> Detalles { get; set; }
    }
}

using Laboratorio1AdmonTIC.Models;

namespace Laboratorio1AdmonTIC.ViewModels
{
    public class CompraConDetallesViewModel
    {
        public Guid compraId {  get; set; }

        public Compras Compra { get; set; }
        public Guid ProveedorId { get; set; }
        public Guid EmpleadoId { get; set; }
        public DateTime FechaCompra { get; set; }
        public double Subtotal {  get; set; }
        public List<DetallesComprasViewModel> Detalles { get; set; }
                
}
}

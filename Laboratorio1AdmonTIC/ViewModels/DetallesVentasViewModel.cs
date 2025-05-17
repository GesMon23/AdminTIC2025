namespace Laboratorio1AdmonTIC.ViewModels
{
    public class DetallesVentasViewModel
    {
        public Guid DetalleVentaId { get; set; }
        public Guid VentasId { get; set; }
        public string? Producto { get; set; }
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Total { get; set; }
        public string? tipoPago { get; set; }
        public Guid? tipoPagoId { get; set; }
        public Guid productoId { get; set; }
    }
}

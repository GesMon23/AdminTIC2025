namespace Laboratorio1AdmonTIC.ViewModels
{
    public class DetallesComprasViewModel
    {
		public Guid DetalleCompraId { get; set; }
		public Guid CompraId { get; set; }
		public string? Producto { get; set; }
		public string? Descripcion { get; set; }
		public int Cantidad {  get; set; }	
		public double PrecioUnitario { get; set; }
		public double Total {  get; set; }
	
		public Guid productoId { get; set; }
    }
}

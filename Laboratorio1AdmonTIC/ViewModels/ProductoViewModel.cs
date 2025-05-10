namespace Laboratorio1AdmonTIC.ViewModels
{
    public class ProductoViewModel
    {
        public Guid Id { get; set; }
        public string Proveedor { get; set; }
        public string Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public float PrecioCompra { get; set; }
        public float PrecioVenta { get; set; }
        public long Stock { get; set; }
        public long StockMinimo { get; set; }
        public string UnidadMedida { get; set; }
    }
}

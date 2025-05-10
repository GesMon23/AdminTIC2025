namespace Laboratorio1AdmonTIC.ViewModels
{
    public class MovimientoInventarioViewModel
    {
        public Guid MovimientoId { get; set; }
        public string ProductoNombre { get; set; }
        public string TipoMovimiento { get; set; }
        public string Empleado { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaCompra { get; set; }
    }
}

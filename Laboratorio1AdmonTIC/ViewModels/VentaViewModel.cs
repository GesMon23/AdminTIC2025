namespace Laboratorio1AdmonTIC.ViewModels
{
    public class VentaViewModel
    {
        public Guid VentasId { get; set; }
        public string Cliente { get; set; }
        public string Empleado { get; set; }
        public string TipoPago { get; set; }
        public DateTime FechaVenta { get; set; }
        public double Total { get; set; }
    }
}

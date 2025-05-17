using Laboratorio1AdmonTIC.Models;

namespace Laboratorio1AdmonTIC.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalComprasHoy { get; set; }
        public decimal TotalVentasHoy { get; set; }
        public int TotalStockDisponible { get; set; }
        public int TotalStockAgotado { get; set; }

        public List<ProductoStockMinimoDto> ProductosStockMinimo { get; set; }
        public List<MovimientoRecienteDto> MovimientosRecientes { get; set; }
    }

    public class ProductoStockMinimoDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public long Stock { get; set; }
        public long StockMinimo { get; set; }
    }

    public class MovimientoRecienteDto
    {
        public string TipoMovimiento { get; set; }
        public double Total { get; set; }
        public DateTime Fecha { get; set; }
    }

}

namespace Laboratorio1AdmonTIC.ViewModels
{
    public class EstadisticasViewModel
    {
        public decimal? totalVentas { get; set; }
        public decimal? difVentas { get; set; }
        public int? unidadesVendidas { get; set; }
        public decimal? difUvendidas { get; set; }
        public int? numeroVentas { get; set; }
        public decimal? difNumeroVentas { get; set; }
        public MasVendidoViewModel MasVendido { get; set; } = new MasVendidoViewModel();
        public ComparativoMetodos MetodoMasUsado { get; set; } = new ComparativoMetodos();

        public class MasVendidoViewModel
        {
            public string? empleado { get; set; }
            public string? productoVendido { get; set; }
        }
        public class ComparativoMetodos
        {
            public string? metodo { get; set; }
            public decimal? metodoporcentual { get; set; }
        }
    }
}

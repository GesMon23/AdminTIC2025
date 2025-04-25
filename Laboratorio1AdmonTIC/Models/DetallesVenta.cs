using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
    public class DetallesVenta
    {
        [Key]
        public Guid DetalleVentaId { get; set; }

        [ForeignKey("VentasId")]
        public Guid VentasId { get; set; }
        public virtual Ventas Ventas { get; set; }

        [ForeignKey("ProductoId")]
        public Guid ProductoId { get; set; }
        public virtual Productos Productos { get; set; }

        [DisplayName("Cantidad")]
        public int Cantidad { get; set; }

        [DisplayName("Precio Unitario")]
        public double PrecioUnitario { get; set; }

        [DisplayName("Total")]
        public double Total { get; set; }

        [ScaffoldColumn(false)]
        public bool Inactivo { get; set; }
    }
}

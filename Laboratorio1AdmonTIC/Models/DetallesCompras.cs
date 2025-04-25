using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
    public class DetallesCompras
    {
        [Key]
        public Guid DetalleCompraId { get; set; }

        [ForeignKey("CompraId")]
        public Guid CompraId { get; set; }
        public virtual Compras Compras { get; set; }

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

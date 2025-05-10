using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
    public class Inventario
    {
        [Key]
        public Guid MovimientoId { get; set; }

        [ForeignKey("ProductoId")]
        public Guid ProductoId { get; set; }
        //public virtual Productos Productos { get; set; }

        [ForeignKey("TipoMovimientoId")]
        public Guid TipoMovimientoId { get; set; }
        //public virtual TiposMovimiento TiposMovimiento { get; set; }

        [ForeignKey("EmpleadosId")]
        public Guid EmpleadosId { get; set; }
        //public virtual Empleados Empleados { get; set; }

        [DisplayName("Cantidad")]
        public int Cantidad { get; set; }

        [DisplayName("FechaMovimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaCompra { get; set; }

        [ScaffoldColumn(false)]
        public bool Inactivo { get; set; }

    }
}

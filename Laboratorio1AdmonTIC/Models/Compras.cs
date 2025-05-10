using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
    public class Compras
    {
        [Key]
        public Guid CompraId { get; set; }

        [ForeignKey("ProveedorId")]
        public Guid ProveedorId { get; set; }
        //public virtual Proveedores Proveedores { get; set; }

        [ForeignKey("EmpleadosId")]
        public Guid EmpleadosId { get; set; }
        //public virtual Empleados Empleados { get; set; }

        [DisplayName("FechaCompra")]
        [DataType(DataType.Date)]
        public DateTime FechaCompra { get; set; }

        [DisplayName("Total")]
        public double Total { get; set; }

        [ScaffoldColumn(false)]
        public bool Inactivo { get; set; }

    }
}

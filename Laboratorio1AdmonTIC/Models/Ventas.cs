using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
    public class Ventas
    {
        [Key]
        public Guid VentasId { get; set; }

        [ForeignKey("ClienteId")]
        public Guid ClienteId { get; set; }
        //public virtual Clientes Clientes { get; set; }

        [ForeignKey("EmpleadosId")]
        public Guid EmpleadosId { get; set; }
        //public virtual Empleados Empleados { get; set; }

        [ForeignKey("MetodoId")]
        public Guid MetodoId { get; set; }
        //public virtual MetodosPago MetodosPago { get; set; }

        [DisplayName("FechaVenta")]
        [DataType(DataType.Date)]
        public DateTime FechaVenta { get; set; }

        [DisplayName("Total")]
        public double Total { get; set; }

        

        [ScaffoldColumn(false)]
        public bool Inactivo { get; set; }
    }
}

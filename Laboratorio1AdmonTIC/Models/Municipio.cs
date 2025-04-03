using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio1AdmonTIC.Models
{
    public class Municipio
    {
        //PRUEBA COMMIT GRUPAL DARWING
        [Key]
        public Guid MunicipioId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string? Nombre {  get; set; }

        [DisplayName("Codigo de Municipio")]
        public int Codigo { get; set; }

        [ForeignKey("DepartamentoId")]
        public Guid? DepartamentoId { get; set; }
        public virtual Departamento? Departamento { get; set;}

        [ScaffoldColumn(false)]
        public bool Inactivo { get; set; }
    }
}

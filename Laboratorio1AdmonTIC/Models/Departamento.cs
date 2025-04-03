using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio1AdmonTIC.Models
{
	public class Departamento
	{
		//Agregar la PK
		[Key]
		public Guid DepartamentoId { get; set; }

		[DisplayName("Nombre de Departamento")]
		public string? Nombre { get; set; }

		[DisplayName("Codigo de Departamento")]
		public int Codigo { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo {  get; set; }

	}
}

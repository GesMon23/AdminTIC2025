using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
	public class MetodosPago
	{

		[Key]
		public Guid MetodoId { get; set; }

		[DisplayName("Tipo de Metodo")]
		public string TipoPago { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }
	}
}

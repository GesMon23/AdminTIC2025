using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
	public class TiposMovimiento
	{
		[Key]
		public Guid TipoMovimientoId { get; set; }

		[DisplayName("Tipo de Movimientos")]
		public string TipoMovimiento{ get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }

	}
}

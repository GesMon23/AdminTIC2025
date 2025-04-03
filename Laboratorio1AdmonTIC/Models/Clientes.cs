using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
	public class Clientes
	{
		[Key]
		public Guid EmpleadoId { get; set; }

		[DisplayName("Nombres")]
		public string Nombres { get; set; }

		[DisplayName("Apellidos")]
		public string Apellidos { get; set; }

		[DisplayName("Telefono")]
		public string? Telefono { get; set; }

		[DisplayName("Direccion")]
		public string Direccion { get; set; }

		[DisplayName("Email")]
		public string? Email { get; set; }

		[DisplayName("CUI")]
		public string CUI { get; set; }

		[DisplayName("NIT")]
		public string NIT { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }
	}
}

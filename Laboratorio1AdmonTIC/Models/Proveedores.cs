using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
	public class Proveedores
	{
		[Key]
		public Guid ProveedorId { get; set; }

		[DisplayName("Nombre")]
		public string Nombre { get; set; }

		[DisplayName("Telefono")]
		public string Telefono { get; set; }

		[DisplayName("Direccion")]
		public string Direccion { get; set; }

		[DisplayName("Email")]
		public string Email { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }
	}
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio1AdmonTIC.Models
{
	public class Empleados
	{
		[Key]
		public Guid EmpleadosId { get; set; }

		[DisplayName("Nombres")]
		public string Nombres { get; set; }

		[DisplayName("Apellidos")]
		public string Apellidos { get; set; }

		[DisplayName("Cargo")]
		public string Cargo { get; set; }

		[DisplayName("Telefono")]
		public string Telefono { get; set; }

		[DisplayName("Email")]
		public string? Email { get; set; }

		[DisplayName("Salario")]
		public float Salario { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }

	}
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
	public class CategoriasProducto
	{
		[Key]
		public Guid CategoriaId { get; set; }

		[DisplayName("Nombre")]
		public string Nombre { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }
	}
}

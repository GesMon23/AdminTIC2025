using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Laboratorio1AdmonTIC.Models
{
	public class CategoriasProducto
	{
		[Key]
		public Guid CategoriaId { get; set; }

		[DisplayName("Nombre")]
		[Required(ErrorMessage = "El nombre es obligatorio.")]
		public string Nombre { get; set; }

		[ScaffoldColumn(false)]
		public bool Inactivo { get; set; }
	}
}

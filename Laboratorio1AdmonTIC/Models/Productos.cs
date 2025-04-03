using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio1AdmonTIC.Models
{
	public class Productos
	{
		[Key]
		public Guid ProductoId { get; set; }

		[ForeignKey("CategoriaId")]
		public Guid CategoriaId { get; set; }
		public virtual CategoriasProducto CategoriasProducto { get; set; }

		[ForeignKey("ProveedorId")]
		public Guid ProveedorId { get; set; }
		public virtual Proveedores Proveedores { get; set; }

		[DisplayName("Nombre")]
		public string Nombre { get; set; }

		[DisplayName("Descripcion")]
		public string Descripcion { get; set; }

		[DisplayName("PrecioCompra")]
		public float PrecioCompra { get; set; }

		[DisplayName("PrecioVenta")]
		public float PrecioVenta { get; set; }

		[DisplayName("Stock")]
		public long Stock { get; set; }

		[DisplayName("StockMinimo")]
		public long StockMinimo { get; set; }

		[DisplayName("UnidadMedida")]
		public string? UnidadMedida { get; set; }
	}
}

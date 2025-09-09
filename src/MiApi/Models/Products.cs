using System;

namespace MiApi.Models
{
	public class Products
	{
		public int IdProducto { get; set; }
		public string Nombre { get; set; } = String.Empty;
		public decimal PrecioUnitario { get; set; }
		public decimal Impuesto { get; set; }

	}
}

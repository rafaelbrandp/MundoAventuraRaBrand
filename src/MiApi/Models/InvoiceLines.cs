using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace MiApi.Models
{
	public class InvoiceLines
	{
		[Key]
		public int IdPedido { get; set; }
		public int IdCliente { get; set; }
		public DateTime? FechaPedido { get; set; }

		public string Estado { get; set; }
		public string Moneda { get; set; }
		public decimal? SubtotalSinImpuestos { get; set; }
		public decimal? TotalImpuestos { get; set; }
		public decimal? CostoEnvio { get; set; }
		public decimal? Descuento { get; set; }
		public decimal? TotalPedido { get; set; }

		public Customers Cliente { get; set; }

		// Colección de ítems de la factura
		public virtual ICollection<InvoiceLinesProducts> Items { get; set; }
	}

	public class InvoiceLinesProducts
	{
		public int Id { get; set; }
		public int IdPedido { get; set; }
		public int IdProducto { get; set; }
		public int Cantidad { get; set; }
		public decimal? PrecioUnitario { get; set; }
		public decimal? Impuesto { get; set; }
	}
}

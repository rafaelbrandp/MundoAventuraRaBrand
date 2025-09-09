using System.Security.Policy;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MiApi.Models
{
	public class Invoices
	{
		[Key]
		public int IdFactura { get; set; }
		public string NumeroFactura { get; set; }
		public DateTime? FechaFactura { get; set; }
		public int IdCliente { get; set; }
		public string Estado { get; set; }
		public string Moneda { get; set; }
		public decimal? SubtotalSinImpuestos { get; set; }
		public decimal? TotalImpuestos { get; set; }
		public decimal? CostoEnvio { get; set; }
		public decimal? Descuento { get; set; }
		public decimal? TotalFactura { get; set; }

		public Customers Cliente { get; set; }

		// Colección de ítems de la factura
		public virtual ICollection<InvoicesProducts> Items { get; set; }
	}

	public class InvoicesProducts
	{
		public int Id { get; set; }
		public int IdFactura { get; set; }
		public int IdProducto { get; set; }
		public int Cantidad { get; set; }
		public decimal? PrecioUnitario { get; set; }
		public decimal? Impuesto { get; set; }
	}
}

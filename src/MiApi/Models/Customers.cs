using System.ComponentModel.DataAnnotations;

namespace MiApi.Models
{
	public class Customers
	{
		[Key]
		public int IdCliente { get; set; }
		public string TipoIdentificacion { get; set; }
		public string NumeroIdentificacion { get; set; }
		public string Nombres { get; set; }
		public string? Email { get; set; }
		public string? Telefono { get; set; }
		public string? Direccion { get; set; }
		public string? Ciudad { get; set; }
		public string? Pais { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models
{
	public class Customer
	{
		public int IdCliente { get; set; }
		public string TDefaultipoIdentificacion { get; set; }
		public string NumeroIdentificacion { get; set; }
		public string Nombres { get; set; }
		public string Email { get; set; }
		public string Telefono { get; set; }
		public string Direccion { get; set; }
		public string Ciudad { get; set; }
		public string Pais { get; set; }
	}
}

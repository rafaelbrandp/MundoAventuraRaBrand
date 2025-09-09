using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models
{
	public class Product
	{
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
    }
}
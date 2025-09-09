using MiApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiApi.Repositorio
{
	public interface IRepositorioFactura
	{
		Task<Invoices> GetFacturaAsync(string numFac);
		Task<Invoices> AgregarFacturaAsync(Invoices factura);
		Task<Invoices> ActualizarFacturaAsync(Invoices factura);
		Task<bool> BorrarFacturaAsync(string numFactura);
	}
}

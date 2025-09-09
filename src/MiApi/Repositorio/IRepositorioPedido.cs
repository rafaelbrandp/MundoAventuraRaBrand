using MiApi.Models;
using System.Threading.Tasks;

namespace MiApi.Repositorio
{
	public interface IRepositorioPedido
	{
		Task<InvoiceLines> GetPedidoAsync(int id);
		Task<InvoiceLines> AgregarPedidoAsync(InvoiceLines pedido);
		Task<InvoiceLines> ActualizarPedidoAsync(InvoiceLines pedido);
		Task<bool> BorrarPedidoAsync(int id);
	}
}

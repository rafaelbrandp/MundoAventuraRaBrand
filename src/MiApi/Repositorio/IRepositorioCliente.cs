using MiApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiApi.Repositorio
{
	public interface IRepositorioCliente
	{

		// Métodos asíncronos
		Task<Customers> GetClienteAsync(int id);
		Task<IEnumerable<Customers>> GetClientesAsync();
		Task<Customers> AgregarClienteAsync(Customers cliente);
		Task<Customers> ActualizarClienteAsync(Customers cliente);
		Task<bool> BorrarClienteAsync(int id);


		// Métodos síncronos
		Customers GetCliente(int id);
		List<Customers> GetClientes();
		Customers AgregarCliente(Customers cliente);
		Customers ActualizarCliente(Customers cliente);
		void BorrarCliente(int id);


	}
}

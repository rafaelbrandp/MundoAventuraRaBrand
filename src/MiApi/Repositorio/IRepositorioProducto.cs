using MiApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiApi.Repositorio
{
	public interface IRepositorioProducto
	{
		Task<Products> GetProductoAsync(int id);
		Task<IEnumerable<Products>> GetProductosAsync();
		Task<Products> AgregarProductoAsync(Products producto);
		Task<Products> ActualizarProductoAsync(Products producto);
		Task<bool> BorrarProductoAsync(int id);
	}
}

using MiApi.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System;
using System.Data;
using MiApi.Repositorio;
using System.Threading.Tasks;


namespace MiApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : Controller
	{
		private readonly IRepositorioProducto _repoProducto;

		public ProductsController(IRepositorioProducto repoProducto)
		{
			_repoProducto = repoProducto;
		}


		//Traer un Producto
		// GET: api/Products/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetProducto(int id)
		{
			try
			{
				var producto = await _repoProducto.GetProductoAsync(id);

				if (producto == null)
				{
					return NotFound($"Producto con ID {id} no encontrado");
				}

				return Ok(producto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Traer todos los Productos
		// GET: api/Products
		[HttpGet]
		public async Task<IActionResult> GetProductos()
		{
			try
			{
				var clientes = await _repoProducto.GetProductosAsync();

				if (clientes == null)
				{
					return NotFound($"Productos no encontrados");
				}

				return Ok(clientes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Crea un Producto
		// POST: api/Products
		[HttpPost]
		public async Task<IActionResult> PostProducto([FromBody] Products producto)
		{
			try
			{
				var clientes = await _repoProducto.AgregarProductoAsync(producto);

				if (clientes.IdProducto == 0)
				{
					return NotFound($"El Producto no fue ingresado");
				}

				return Ok(clientes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Actualiza un Producto
		// PUT: api/Products
		[HttpPost("Update")]
		public async Task<IActionResult> PutProducto([FromBody] Products producto)
		{
			try
			{
				var clientes = await _repoProducto.ActualizarProductoAsync(producto);

				if (clientes.IdProducto == 0)
				{
					return NotFound($"El Producto no fue actualizado");
				}

				return Ok(clientes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Eliminar un Producto
		// DELETE: api/Products/Delete/2
		[HttpPost("Delete/{id}")]
		public async Task<IActionResult> DeleteProducto(int id)
		{
			try
			{
				var eliminado = await _repoProducto.BorrarProductoAsync(id);

				if (!eliminado)
				{
					return NotFound($"Producto con ID {id} no encontrado");
				}

				return NoContent(); // 204 No Content es la respuesta estándar para DELETE exitoso
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}



		//Actualiza un Producto
		// PUT: api/Products
		//[HttpPut]

		//Eliminar un Producto
		// DELETE: api/Products
		//[HttpDelete("{id}")]
	}
}

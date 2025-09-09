using MiApi.Models;
using MiApi.Repositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MiApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : Controller
	{
		private readonly IRepositorioCliente _repoCliente;

		public CustomersController(IRepositorioCliente repoCliente)
		{
			_repoCliente = repoCliente;
		}


		//Traer un CLiente
		// GET: api/Customers/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetCliente(int id)
		{
			try
			{
				var cliente = await _repoCliente.GetClienteAsync(id);

				if (cliente == null)
				{
					return NotFound($"Cliente con ID {id} no encontrado");
				}

				return Ok(cliente);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Traer todos los CLientes
		// GET: api/Customers
		[HttpGet]
		public async Task<IActionResult> GetClientes()
		{
			try
			{
				var clientes = await _repoCliente.GetClientesAsync();

				if (clientes == null)
				{
					return NotFound($"Clientes no encontrados");
				}

				return Ok(clientes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Crea un CLiente
		// POST: api/Customers
		[HttpPost]
		public async Task<IActionResult> PostCliente([FromBody] Customers cliente)
		{
			try
			{
				var clientes = await _repoCliente.AgregarClienteAsync(cliente);

				if (clientes.IdCliente == 0)
				{
					return NotFound($"El Cliente no fue ingresado");
				}

				return Ok(clientes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}

		//Actualiza un CLiente
		// PUT: api/Customers/Update
		[HttpPost("Update")]
		public async Task<IActionResult> PutCliente([FromBody] Customers cliente)
		{
			try
			{
				var clientes = await _repoCliente.ActualizarClienteAsync(cliente);

				if (clientes.IdCliente == 0)
				{
					return NotFound($"El Cliente no fue actualizado");
				}

				return Ok(clientes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Eliminar un CLiente
		// DELETE: api/Customers/Delete
		[HttpPost("Delete/{id}")]
		public async Task<IActionResult> DeleteCliente(int id)
		{
			try
			{
				var eliminado = await _repoCliente.BorrarClienteAsync(id);

				if (!eliminado)
				{
					return NotFound($"Cliente con ID {id} no encontrado");
				}

				return NoContent(); // 204 No Content es la respuesta estándar para DELETE exitoso
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}



		//Eliminar un CLiente
		// DELETE: api/Customers
		//[HttpDelete("{id}")]     Modificado para que funcionara con MVC 4.8

		//Actualiza un CLiente
		// PUT: api/Customers
		//[HttpPut]                Modificado para que funcionara con MVC 4.8



		/* Metodo sincrono
		// GET: api/Customers/5
		[HttpGet("{id}")]
		public IActionResult LeerCliente(int id)
		{
			return Ok(_repoCliente.GetCliente(id));
		}
		// GET: api/Customers
		[HttpGet]
		public IActionResult LeerClientes()
		{
			return Ok(_repoCliente.GetClientes());
		}
		*/

	}
}

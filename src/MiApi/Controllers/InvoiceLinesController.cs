using MiApi.Repositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MiApi.Models;

namespace MiApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvoiceLinesController : ControllerBase
	{
		private readonly IRepositorioPedido _repoPedido;

		public InvoiceLinesController(IRepositorioPedido repoPedido)
		{
			_repoPedido = repoPedido;
		}

		//Traer un Pedido
		// GET: api/InvoiceLines/1
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPedido(int id)
		{
			try
			{
				var pedido = await _repoPedido.GetPedidoAsync(id);

				if (pedido == null)
				{
					return NotFound($"Pedido numero {id} no encontrado");
				}

				return Ok(pedido);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Crea un Pedido
		// POST: api/InvoiceLines
		[HttpPost]
		public async Task<IActionResult> PostPedido([FromBody] InvoiceLines pedido)
		{
			try
			{
				var pedidoR = await _repoPedido.AgregarPedidoAsync(pedido);

				if (pedidoR.IdPedido == 0)
				{
					return NotFound($"El Pedido no fue ingresado");
				}

				return Ok(pedidoR);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Actualiza un Pedido
		// PUT: api/InvoiceLines
		[HttpPut]
		public async Task<IActionResult> PutPedido([FromBody] InvoiceLines pedido)
		{
			try
			{
				var pedidoResp = await _repoPedido.ActualizarPedidoAsync(pedido);

				if (pedidoResp == null)
				{
					return NotFound($"El pedido con ID {pedido.IdPedido} no existe");
				}
				var pedidoUp = await _repoPedido.GetPedidoAsync(pedidoResp.IdPedido);

				return Ok(pedidoUp);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Eliminar un Pedido
		// DELETE: api/InvoiceLines
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePedido(int id)
		{
			try
			{
				var eliminado = await _repoPedido.BorrarPedidoAsync(id);

				if (!eliminado)
				{
					return NotFound($"El pedido numero {id} no encontrado");
				}

				return NoContent(); // 204 No Content es la respuesta estándar para DELETE exitoso
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}




	}
}

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
	public class InvoicesController : ControllerBase
	{
		private readonly IRepositorioFactura _repoFactura;

		public InvoicesController(IRepositorioFactura repoFactura)
		{
			_repoFactura = repoFactura;
		}


		//Traer una Factura
		// GET: api/Invoices/001
		[HttpGet("{numFact}")]
		public async Task<IActionResult> GetFactura(string numFact)
		{
			try
			{
				var factura = await _repoFactura.GetFacturaAsync(numFact);

				if (factura == null)
				{
					return NotFound($"Factura numero {numFact} no encontrada");
				}

				return Ok(factura);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Crea una Factura
		// POST: api/Invoices
		[HttpPost]
		public async Task<IActionResult> PostFactura([FromBody] Invoices factura)
		{
			try
			{
				var facturaR = await _repoFactura.AgregarFacturaAsync(factura);

				if (facturaR.IdFactura == 0)
				{
					return NotFound($"La Factura no fue ingresada");
				}

				return Ok(facturaR);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Actualiza una Factura
		// PUT: api/Invoices
		[HttpPut]
		public async Task<IActionResult> PutFactura([FromBody] Invoices factura)
		{
			try
			{
				var facturaResp = await _repoFactura.ActualizarFacturaAsync(factura);

				if (facturaResp == null)
				{
					return NotFound($"La Factura con ID {factura.IdFactura} no existe");
				}
				var facturaUp = await _repoFactura.GetFacturaAsync(facturaResp.NumeroFactura);

				return Ok(facturaUp);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error interno del servidor");
			}
		}


		//Eliminar una factura
		// DELETE: api/Invoices
		[HttpDelete("{numFact}")]
		public async Task<IActionResult> DeleteFactura(string numFact)
		{
			try
			{
				var eliminado = await _repoFactura.BorrarFacturaAsync(numFact);

				if (!eliminado)
				{
					return NotFound($"Factura numero {numFact} no encontrada");
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

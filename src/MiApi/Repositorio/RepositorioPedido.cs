using MiApi.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MiApi.Repositorio
{
	public class RepositorioPedido : IRepositorioPedido
	{
		private readonly IDbConnection _bd;

		public RepositorioPedido(IConfiguration configuration)
		{
			_bd = new SqlConnection(configuration.GetConnectionString("ConexionSQLLocalDB"));
		}

		// Métodos asíncronos
		//Traer un Pedido
		public async Task<InvoiceLines> GetPedidoAsync(int id)
		{
			// Consulta principal para obtener el pedido (encabezado)
			var sql = @"SELECT IdPedido, IdCliente, FechaPedido,  Estado, Moneda, 
                SubtotalSinImpuestos, TotalImpuestos, CostoEnvio, Descuento, TotalPedido 
                FROM InvoiceLines 
                WHERE IdPedido = @idPedido";

			var pedido = await _bd.QuerySingleOrDefaultAsync<InvoiceLines>(sql, new { IdPedido = id });

			if (pedido == null)
				return null;

			// Consulta para obtener los items del pedido
			var sql2 = @"SELECT Id, IdPedido, IdProducto, Cantidad, PrecioUnitario, Impuesto 
                 FROM InvoiceLinesProducts 
                 WHERE IdPedido = @IdPedido";

			var items = await _bd.QueryAsync<InvoiceLinesProducts>(sql2, new { IdPedido = pedido.IdPedido });
			pedido.Items = items.ToList();

			// Consulta para obtener información del cliente
			var sql3 = @"SELECT IdCliente, TipoIdentificacion, NumeroIdentificacion, Nombres, 
                 Email, Telefono, Direccion, Ciudad, Pais 
                 FROM Customers 
                 WHERE IdCliente = @IdCliente";

			pedido.Cliente = await _bd.QuerySingleOrDefaultAsync<Customers>(sql3, new { IdCliente = pedido.IdCliente });

			return pedido;
		}


		//Crear un Pedido
		public async Task<InvoiceLines> AgregarPedidoAsync(InvoiceLines pedido)
		{
			try
			{
				// Crea Pedido (encabezado)
				var sql = "INSERT INTO InvoiceLines (IdCliente, FechaPedido, Estado, Moneda,"
				+ " SubtotalSinImpuestos, TotalImpuestos, CostoEnvio, Descuento, TotalPedido)"
				+ " VALUES (@IdCliente, @FechaPedido, @Estado, @Moneda, "
				+ " @SubtotalSinImpuestos, @TotalImpuestos, @CostoEnvio, @Descuento, @TotalPedido)"
				+ " SELECT CAST(SCOPE_IDENTITY() AS INT);  ";
				var id = await _bd.ExecuteScalarAsync<int>(sql, new
				{
					pedido.IdCliente,
					pedido.FechaPedido,
					pedido.Estado,
					pedido.Moneda,
					pedido.SubtotalSinImpuestos,
					pedido.TotalImpuestos,
					pedido.CostoEnvio,
					pedido.Descuento,
					pedido.TotalPedido
				});

				if (id == 0)
					return null;

				pedido.IdPedido = id;

				// Crea Items de Pedido
				if (pedido.Items != null)
				{
					
					var idItem = 0;
					foreach (var item in pedido.Items)
					{
						sql = "INSERT INTO InvoiceLinesProducts (IdPedido, IdProducto, Cantidad, PrecioUnitario, Impuesto) "
							+ " VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario, @Impuesto) ";

						idItem = await _bd.ExecuteScalarAsync<int>(sql, new
						{
							pedido.IdPedido,
							item.IdProducto,
							item.Cantidad,
							item.PrecioUnitario,
							item.Impuesto
						});
						item.IdPedido = pedido.IdPedido;
						item.Id = idItem;
					}
				}

				return pedido;

			}
			catch (Exception ex)
			{
				throw new Exception("Error al agregar el pedido: " + ex.Message);
			}
		}

		//Actualizar un Pedido
		public async Task<InvoiceLines> ActualizarPedidoAsync(InvoiceLines pedido)
		{
			//Se verififca si el pedido existe
			var sqlV = "SELECT IdPedido FROM InvoiceLines WHERE IdPedido = @IdPedido;";
			var pedidoUp = await _bd.QuerySingleOrDefaultAsync<InvoiceLines>(sqlV, new { IdPedido = pedido.IdPedido });
			if (pedidoUp == null)
				return pedidoUp;


			// Crea script para actualizar InvoiceLines (encabezado del pedido)
			var sql = "UPDATE InvoiceLines SET "
			+ "IdCliente=@IdCliente, FechaPedido=@FechaPedido, "
			+ "Estado=@Estado, Moneda=@Moneda, SubtotalSinImpuestos=@SubtotalSinImpuestos, "
			+ "TotalImpuestos=@TotalImpuestos, CostoEnvio=@CostoEnvio, Descuento=@Descuento, "
			+ "TotalPedido=@TotalPedido "
			+ "WHERE IdPedido = @IdPedido;  ";

			// Crea script para aborrar InvoiceLinesProducts (los Items del pedido)
			sql += "DELETE FROM InvoiceLinesProducts WHERE IdPedido = @IdPedido; ";

			// Crea script para adicionar InvoiceLinesProducts (los Items del pedido)
			if (pedido.Items != null)
			{
				sql += "INSERT INTO InvoiceLinesProducts (IdPedido, IdProducto, Cantidad, PrecioUnitario, Impuesto)"
						+ " VALUES ";
				foreach (var item in pedido.Items)
				{
					sql += "(@IdPedido, " + item.IdProducto + ", " + Convert.ToString(item.Cantidad).Replace(",", ".") + ", "
						+ Convert.ToString(item.PrecioUnitario).Replace(",", ".") + ", " + Convert.ToString(item.Impuesto).Replace(",", ".") + "), ";
				}

				sql = sql.Remove(sql.Length - 2) + ";  ";
			}

			sql += "SELECT IdPedido FROM InvoiceLines WHERE IdPedido = @IdPedido;  ";

			pedidoUp = await _bd.QuerySingleOrDefaultAsync<InvoiceLines>(sql, new
			{
				IdPedido = pedido.IdPedido,
				IdCliente = pedido.IdCliente,
				FechaPedido = pedido.FechaPedido,
				Estado = pedido.Estado,
				Moneda = pedido.Moneda,
				SubtotalSinImpuestos = pedido.SubtotalSinImpuestos,
				TotalImpuestos = pedido.TotalImpuestos,
				CostoEnvio = pedido.CostoEnvio,
				Descuento = pedido.Descuento,
				TotalPedido = pedido.TotalPedido
			});

			if (pedidoUp.IdPedido == pedido.IdPedido)
			{
				return pedido;
			}
			else
			{
				return null;
			}
		}


		//Eliminar un Pedido
		public async Task<bool> BorrarPedidoAsync(int id)
		{
			// Consulta principal para obtener el pedido
			var sql = @"SELECT IdPedido "
				+ " FROM InvoiceLines WHERE IdPedido = @IdPedido";

			var pedido = await _bd.QuerySingleOrDefaultAsync<InvoiceLines>(sql, new { IdPedido = id });

			if (pedido == null)
				return false;

			sql = "DELETE FROM InvoiceLinesProducts WHERE IdPedido = @IdPedido";
			var filasAfectadasItem = await _bd.ExecuteAsync(sql, new { IdPedido = pedido.IdPedido});

			sql = "DELETE FROM InvoiceLines WHERE IdPedido = @IdPedido";
			var filasAfectadas = await _bd.ExecuteAsync(sql, new { IdPedido = pedido.IdPedido });

			return filasAfectadas > 0;
		}


	}
}

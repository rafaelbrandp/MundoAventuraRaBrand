using MiApi.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;


namespace MiApi.Repositorio
{
	public class RepositorioFactura : IRepositorioFactura
	{
		private readonly IDbConnection _bd;

		public RepositorioFactura(IConfiguration configuration)
		{
			_bd = new SqlConnection(configuration.GetConnectionString("ConexionSQLLocalDB"));
		}



		// Métodos asíncronos
		//Traer una Factura
		public async Task<Invoices> GetFacturaAsync(string numFactura)
		{
			// Consulta principal para obtener la factura
			var sql = @"SELECT IdFactura, NumeroFactura, FechaFactura, IdCliente, Estado, Moneda, 
                SubtotalSinImpuestos, TotalImpuestos, CostoEnvio, Descuento, TotalFactura 
                FROM Invoices 
                WHERE NumeroFactura = @NumeroFactura";

			var factura = await _bd.QuerySingleOrDefaultAsync<Invoices>(sql, new { NumeroFactura = numFactura });

			if (factura == null)
				return null;

			// Consulta para obtener los items de la factura
			var sql2 = @"SELECT Id, IdFactura, IdProducto, Cantidad, PrecioUnitario, Impuesto 
                 FROM InvoicesProducts 
                 WHERE IdFactura = @IdFactura";

			var items = await _bd.QueryAsync<InvoicesProducts>(sql2, new { IdFactura = factura.IdFactura });
			factura.Items = items.ToList();

			// Consulta para obtener información del cliente
			var sql3 = @"SELECT IdCliente, TipoIdentificacion, NumeroIdentificacion, Nombres, 
                 Email, Telefono, Direccion, Ciudad, Pais 
                 FROM Customers 
                 WHERE IdCliente = @IdCliente";

			factura.Cliente = await _bd.QuerySingleOrDefaultAsync<Customers>(sql3, new { IdCliente = factura.IdCliente });

			return factura;
		}


		//Crear una Factura
		public async Task<Invoices> AgregarFacturaAsync(Invoices factura)
		{
			try
			{
				// Ingresa la factura
				var sql = "INSERT INTO Invoices (NumeroFactura, FechaFactura, IdCliente, Estado, Moneda,"
				+ " SubtotalSinImpuestos, TotalImpuestos, CostoEnvio, Descuento, TotalFactura)"
				+ " VALUES (@NumeroFactura, @FechaFactura, @IdCliente, @Estado, @Moneda,"
				+ " @SubtotalSinImpuestos, @TotalImpuestos, @CostoEnvio, @Descuento, @TotalFactura)"
				+ " SELECT CAST(SCOPE_IDENTITY() AS INT);";
				var id = await _bd.ExecuteScalarAsync<int>(sql, new
				{
					factura.NumeroFactura,
					factura.FechaFactura,
					factura.IdCliente,
					factura.Estado,
					factura.Moneda,
					factura.SubtotalSinImpuestos,
					factura.TotalImpuestos,
					factura.CostoEnvio,
					factura.Descuento,
					factura.TotalFactura
				});
				
				if (id == 0)
					return null;
				
				factura.IdFactura = id;

				// Ingresa los Items
				if (factura.Items != null)
				{
					var idItem = 0;
					foreach (var item in factura.Items)
					{
						sql = "INSERT INTO InvoicesProducts (IdFactura, IdProducto, Cantidad, PrecioUnitario, Impuesto)"
							+ " VALUES (@IdFactura, @IdProducto, @Cantidad, @PrecioUnitario, @Impuesto)"
							+ " SELECT CAST(SCOPE_IDENTITY() AS INT);";
						idItem = await _bd.ExecuteScalarAsync<int>(sql, new
						{
							factura.IdFactura,
							item.IdProducto,
							item.Cantidad,
							item.PrecioUnitario,
							item.Impuesto
						});
						item.IdFactura = factura.IdFactura;
						item.Id = idItem;
					}
				}

				return factura;

			}
			catch (Exception ex)
			{
				throw new Exception("Error al agregar el producto: " + ex.Message);
			}
		}

		
		//Actualizar una Factura
		public async Task<Invoices> ActualizarFacturaAsync(Invoices factura)
		{
			//Se verififca si la factura existe
			var sqlV = "SELECT IdFactura, NumeroFactura FROM Invoices WHERE IdFactura = @IdFactura;";
			var facturaUp = await _bd.QuerySingleOrDefaultAsync<Invoices>(sqlV, new { IdFactura = factura.IdFactura });
			if (facturaUp == null)
				return facturaUp;


			// Crea script para actualizar Invoices (encabezado de la factura)
			var sql = "UPDATE Invoices SET "
			+ "NumeroFactura=@NumeroFactura, FechaFactura=@FechaFactura, IdCliente=@IdCliente, "
			+ "Estado=@Estado, Moneda=@Moneda, SubtotalSinImpuestos=@SubtotalSinImpuestos, "
			+ "TotalImpuestos=@TotalImpuestos, CostoEnvio=@CostoEnvio, Descuento=@Descuento, "
			+ "TotalFactura=@TotalFactura "
			+ "WHERE IdFactura = @IdFactura;  ";

			// Crea script para aborrar InvoicesProducts (los Items de la factura)
			sql += "DELETE FROM InvoicesProducts WHERE IdFactura = @IdFactura; ";

			// Crea script para adicionar InvoicesProducts (los Items de la factura)
			if (factura.Items != null)
			{
				sql += "INSERT INTO InvoicesProducts (IdFactura, IdProducto, Cantidad, PrecioUnitario, Impuesto)"
						+ " VALUES ";
				foreach (var item in factura.Items)
				{
					sql += "(@IdFactura, " + item.IdProducto + ", " + Convert.ToString(item.Cantidad).Replace(",",".") + ", "
						+ Convert.ToString(item.PrecioUnitario).Replace(",", ".") + ", " + Convert.ToString(item.Impuesto).Replace(",", ".") + "), ";
				}

				sql = sql.Remove(sql.Length - 2) + ";  ";
			}

			sql += "SELECT IdFactura, NumeroFactura FROM Invoices WHERE IdFactura = @IdFactura;  ";

			facturaUp = await _bd.QuerySingleOrDefaultAsync<Invoices>(sql, new
			{
				IdFactura = factura.IdFactura,
				NumeroFactura = factura.NumeroFactura,
				FechaFactura = factura.FechaFactura,
				IdCliente = factura.IdCliente,
				Estado = factura.Estado,
				Moneda = factura.Moneda,
				SubtotalSinImpuestos = factura.SubtotalSinImpuestos,
				TotalImpuestos = factura.TotalImpuestos,
				CostoEnvio = factura.CostoEnvio,
				Descuento = factura.Descuento,
				TotalFactura = factura.TotalFactura
			});

			if (facturaUp.IdFactura == factura.IdFactura)
			{
				return factura;
			}
			else
			{
				return null;
			}
		}
	

		//Eliminar una Factura
		public async Task<bool> BorrarFacturaAsync(string numFactura)
		{
			// Consulta principal para obtener la factura
			var sql = @"SELECT IdFactura, NumeroFactura "
                + " FROM Invoices WHERE NumeroFactura = @NumeroFactura";

			var factura = await _bd.QuerySingleOrDefaultAsync<Invoices>(sql, new { NumeroFactura = numFactura });

			if (factura == null)
				return false;

			sql = "DELETE FROM InvoicesProducts WHERE IdFactura = @IdFactura";
			var filasAfectadasItem = await _bd.ExecuteAsync(sql, new { IdFactura = factura.IdFactura });

			sql = "DELETE FROM Invoices WHERE IdFactura = @IdFactura";
			var filasAfectadas = await _bd.ExecuteAsync(sql, new { IdFactura = factura.IdFactura });

			return filasAfectadas > 0;
		}

	}
}

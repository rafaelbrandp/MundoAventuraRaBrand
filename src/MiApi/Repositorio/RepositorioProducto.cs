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
	public class RepositorioProducto : IRepositorioProducto
	{
		private readonly IDbConnection _bd;

		public RepositorioProducto(IConfiguration configuration)
		{
			_bd = new SqlConnection(configuration.GetConnectionString("ConexionSQLLocalDB"));
		}


		// Métodos asíncronos
		//Traer un Producto
		public async Task<Products> GetProductoAsync(int id)
		{
			var sql = "SELECT IdProducto, Nombre, PrecioUnitario, Impuesto "
				+ "FROM Products WHERE IdProducto = @IdProducto";
			return await _bd.QuerySingleOrDefaultAsync<Products>(sql, new { IdProducto = id });
		}

		//Traer todos los Productos
		public async Task<IEnumerable<Products>> GetProductosAsync()
		{
			var sql = "SELECT IdProducto, Nombre, PrecioUnitario, Impuesto FROM Products";
			return await _bd.QueryAsync<Products>(sql);
		}

		//Crear un Producto
		public async Task<Products> AgregarProductoAsync(Products producto)
		{
			try
			{
				var sql = "INSERT INTO Products (Nombre, PrecioUnitario, Impuesto)"
				+ " VALUES (@Nombre, @PrecioUnitario, @Impuesto)"
				+ " SELECT CAST(SCOPE_IDENTITY() AS INT);";
				var id = await _bd.ExecuteScalarAsync<int>(sql, new
				{
					producto.Nombre,
					producto.PrecioUnitario,
					producto.Impuesto
				});

				producto.IdProducto = id;
				return producto;

			}
			catch (Exception ex)
			{
				throw new Exception("Error al agregar el producto: " + ex.Message);
			}
		}

		//Actualiza un Producto
		public async Task<Products> ActualizarProductoAsync(Products producto)
		{
			var sql = "UPDATE Products SET "
				+ "Nombre = @Nombre, PrecioUnitario = @PrecioUnitario, Impuesto = @Impuesto "
				+ "WHERE IdProducto = @IdProducto";

			var filasAfectadas = await _bd.ExecuteAsync(sql, producto);
			if (filasAfectadas > 0)
			{
				return producto;
			}
			else
			{
				return new Products();
			}
		}

		//Eliminar un Producto
		public async Task<bool> BorrarProductoAsync(int id)
		{
			var sql = "DELETE FROM Products WHERE IdProducto = @IdProducto";
			var filasAfectadas = await _bd.ExecuteAsync(sql, new { IdProducto = id });
			return filasAfectadas > 0;
		}

	}
}

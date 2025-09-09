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
	public class RepositorioCliente : IRepositorioCliente
	{
		private readonly IDbConnection _bd;

        public RepositorioCliente(IConfiguration configuration)
        {
			_bd = new SqlConnection(configuration.GetConnectionString("ConexionSQLLocalDB"));
        }


		// Métodos asíncronos
		//Traer un CLiente
		public async Task<Customers> GetClienteAsync(int id)
		{
			var sql = "SELECT IdCliente, TipoIdentificacion, NumeroIdentificacion, Nombres, "
				+ "Email, Telefono, Direccion, Ciudad, Pais FROM Customers WHERE IdCliente = @IdCliente";
			return await _bd.QuerySingleOrDefaultAsync<Customers>(sql, new { IdCliente = id });
		}

		//Traer todos los CLientes
		public async Task<IEnumerable<Customers>> GetClientesAsync()
		{
			var sql = "SELECT IdCliente, TipoIdentificacion, NumeroIdentificacion, Nombres, "
				+ "Email, Telefono, Direccion, Ciudad, Pais FROM Customers";
			return await _bd.QueryAsync<Customers>(sql);
		}

		//Crear un CLiente
		public async Task<Customers> AgregarClienteAsync(Customers cliente)
		{
			try
			{
				var sql = "INSERT INTO Customers (TipoIdentificacion, NumeroIdentificacion, Nombres, Email, Telefono, Direccion, Ciudad, Pais)"
				+ " VALUES (@TipoIdentificacion, @NumeroIdentificacion, @Nombres, @Email, @Telefono, @Direccion, @Ciudad, @Pais)"
				+ " SELECT CAST(SCOPE_IDENTITY() AS INT);";
				var id = await _bd.ExecuteScalarAsync<int>(sql, new
				{
					cliente.TipoIdentificacion,
					cliente.NumeroIdentificacion,
					cliente.Nombres,
					cliente.Email,
					cliente.Telefono,
					cliente.Direccion,
					cliente.Ciudad,
					cliente.Pais
				});

				cliente.IdCliente = id;
				return cliente;

			}
			catch (Exception ex)
			{
				throw new Exception("Error al agregar el cliente: " + ex.Message);
			}
		}

		//Actualiza un CLiente
		public async Task<Customers> ActualizarClienteAsync(Customers cliente)
		{
			var sql = "UPDATE Customers SET "
				+ "TipoIdentificacion = @TipoIdentificacion, NumeroIdentificacion = @NumeroIdentificacion, Nombres = @Nombres, "
				+ "Email = @Email, Telefono = @Telefono, Direccion = @Direccion, Ciudad = @Ciudad, Pais = @Pais "
				+ "WHERE IdCliente = @IdCliente";

			var filasAfectadas = await _bd.ExecuteAsync(sql, cliente);
			if(filasAfectadas > 0)
			{
				return cliente;
			}
			else
			{
				return new Customers();
			}
		}

		//Eliminar un CLiente
		public async Task<bool> BorrarClienteAsync(int id)
		{
			var sql = "DELETE FROM Customers WHERE IdCliente = @IdCliente";
			var filasAfectadas = await _bd.ExecuteAsync(sql, new { IdCliente = id });
			return filasAfectadas > 0;
		}



		// Métodos síncronos
		public Customers GetCliente(int id)
		{
			var sql = "SELECT * FROM Customers WHERE IdCliente = @IdCliente";
			return _bd.Query<Customers>(sql, new { @IdCliente = id }).Single();
		}

		public List<Customers> GetClientes()
		{
			var sql = "SELECT * FROM Customers";
			return _bd.Query<Customers>(sql).ToList();
		}

		public Customers AgregarCliente(Customers cliente)
		{
			var sql = "INSERT INTO Customers (TipoIdentificacion, NumeroIdentificacion, Nombres, Email, Telefono, Direccion, Ciudad, Pais)"
				+ " VALUES (@TipoIdentificacion, @NumeroIdentificacion, @Nombres, @Email, @Telefono, @Direccion, @Ciudad, @Pais)"
				+ " SELECT CAST(SCOPE_IDENTITY() AS INT);";
			var id = _bd.Query<int>(sql, new
			{
				cliente.TipoIdentificacion,
				cliente.NumeroIdentificacion,
				cliente.Nombres,
				cliente.Email,
				cliente.Telefono,
				cliente.Direccion,
				cliente.Ciudad,
				cliente.Pais
			}).Single();

			cliente.IdCliente = id;
			return cliente;
		}

		public Customers ActualizarCliente(Customers cliente)
		{
			var sql = "UPDATE Customers SET "
				+ "TipoIdentificacion = @TipoIdentificacion, NumeroIdentificacion = @NumeroIdentificacion, Nombres = @Nombres, "
				+ "Email = @Email, Telefono = @Telefono, Direccion = @Direccion, Ciudad = @Ciudad, Pais = @Pais "
				+ "WHERE IdCliente = @IdCliente";

			_bd.Execute(sql, cliente);
			return cliente;
		}

		public void BorrarCliente(int id)
		{
			var sql = "DELETE FROM Customers WHERE IdCliente = @IdCliente";
			_bd.Execute(sql, new { @IdCliente = id });
		}

	}
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiApi.Repositorio;

namespace MiApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();


			//Registrar tu repositorio aqui
			services.AddScoped<IRepositorioCliente, RepositorioCliente>();
			services.AddScoped<IRepositorioProducto, RepositorioProducto>();
			services.AddScoped<IRepositorioFactura, RepositorioFactura>();
			services.AddScoped<IRepositorioPedido, RepositorioPedido>();

			// Configurar CORS
			services.AddCors(options =>
			{
				/*
				options.AddPolicy("AllowMvcApp",
					builder =>
					{
						builder.WithOrigins("https://localhost:44314") // URL de tu API
							   .AllowAnyHeader()
							   .AllowAnyMethod();
					});
				*/
				options.AddPolicy("AllowAll",
					builder =>
					{
						builder.AllowAnyOrigin()
							   .AllowAnyHeader()
							   .AllowAnyMethod();
					});
			});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			


			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();


			app.UseDefaultFiles(); // Esto permite servir index.html por defecto
			app.UseStaticFiles(); // Esto sirve archivos estáticos desde wwwroot


			app.UseRouting();

			//app.UseCors("AllowMvcApp"); // Usar la política definida
			app.UseCors("AllowAll");

			app.UseAuthorization();


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers(); // Esto mapea los controladores de API
			});
		}
	}
}

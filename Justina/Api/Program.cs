﻿using Api.Services.Attempts;
using Infraestruture.Persistence.Context;
using Infraestruture.Repository;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Configuramos la cadena de conexión leyendo el appsettings para no fijarla en código
            // y permitir cambiar de base de datos (dev, prod, etc.) sin recompilar.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Registramos el DbContext como servicio para que pueda ser inyectado en repositorios
            // y servicios de aplicación, reutilizando el contexto por petición HTTP.
            builder.Services.AddDbContext<JustinaDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Registramos el repositorio de Attempts como servicio Scoped para encapsular el acceso
            // a datos de los intentos de juegos y poder reutilizar lógica de persistencia.
            builder.Services.AddScoped<IAttemptRepository, AttemptRepository>();

            // Registramos el servicio de aplicación que calcula métricas de intentos y expone
            // operaciones de más alto nivel a los controladores.
            builder.Services.AddScoped<IAttemptService, AttemptService>();

            // Registramos los controladores MVC que expondrán los endpoints HTTP de la API.

            builder.Services.AddControllers();
            // Activamos Swagger/OpenAPI para poder explorar y probar los endpoints desde UI web
            // sin necesidad de un cliente externo como Postman.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

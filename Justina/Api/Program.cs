
using Application.Interface.Repository;
using Infraestruture.Persistence.Context;
using Infraestruture.Repository;
using Microsoft.EntityFrameworkCore;
using Infraestruture.Extensions;
using Application.Extensions;
using Api.Extensions;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           
            // Inyectamos lo de cada capa
            builder.Services.AddInfrastructureServices(builder.Configuration); // Viene de Infrastructure
            builder.Services.AddApplicationLayer();

            builder.Services.AddSwaggerConfiguration();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

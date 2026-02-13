using Application.Interface.Repository;
using Application.Interface.UnitOfWork;
using Infraestruture.Persistence.Context;
using Infraestruture.Repository;
using Infraestruture.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestruture.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registro de DbContext
            services.AddDbContext<JustinaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Registro de Repositorios Genéricos
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Registro de Repositorios Específicos
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();      // 👈 AGREGADO
            services.AddScoped<IAttemptRepository, AttemptRepository>();

            // Registro de UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
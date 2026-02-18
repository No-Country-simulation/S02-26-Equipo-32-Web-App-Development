using Application.Interface.Repository;
using Application.Interface.Repository.Tests;
using Application.Interface.Service.UsersRoles;
using Application.Interface.UnitOfWor;
using Application.Service.UsersRoles;
using Infraestruture.Persistence.Context;
using Infraestruture.Persistence.Seeds;
using Infraestruture.Repository;
using Infraestruture.Repository.Tests;
using Infraestruture.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infraestruture.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registro de DbContext
            services.AddDbContext<JustinaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Registro de Repositorios
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<IAttemptRepository, AttemptRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DbSeeder>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }
    }
}

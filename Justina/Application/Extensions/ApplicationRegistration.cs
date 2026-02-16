
using Application.Interface.Service;
using Application.Mapping.Roles;
using Application.Service.Attempts;
using Application.Service.Roles;
using Application.Service.Users;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class ApplicationRegistration 
    {
      
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // Registra AutoMapper buscando los Profiles en este ensamblado
            services.AddScoped<IRoleService, RoleService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Registro de Servicios de Aplicación
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAttemptService, AttemptService>();

            // Application/Extensions/ApplicationRegistration.cs
            services.AddAutoMapper(typeof(RoleProfile).Assembly);
            


            return services;
        }
    }
}

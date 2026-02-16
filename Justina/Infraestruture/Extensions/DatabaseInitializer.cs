using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestruture.Extensions
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JustinaDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<JustinaDbContext>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            try
            {
                await context.Database.MigrateAsync();

                // 1. Creacion de Roles por defecto (Solo si no existen)
                if (!await context.Roles.AnyAsync())
                {
                    logger.LogInformation("Creando roles del sistema...");

                    var roles = new[]
                    {
                        new Role
                        {
                            Name = "Admin",
                            Description = "Administrador del sistema - Acceso Total",
                            IsActive = true,
                            CreatedBy = "system",
                            CreatedAt = DateTime.UtcNow,
                        },
                        new Role
                        {
                            Name = "User",  // 👈 NUEVO ROL
                            Description = "Usuario registrado - Acceso gratuito limitado",
                            IsActive = true,
                            CreatedBy = "system",
                            CreatedAt = DateTime.UtcNow
                        },
                        new Role
                        {
                            Name = "Surgeon",
                            Description = "Cirujano - Puede realizar simulaciones y dar feedback",
                            IsActive = true,
                            CreatedBy = "system",
                            CreatedAt = DateTime.UtcNow
                        },
                        new Role
                        {
                            Name = "Observer",
                            Description = "Observador - Puede ver reportes y métricas",
                            IsActive = true,
                            CreatedBy = "system",
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.Roles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Roles creados: Admin, User, Surgeon, Observer");
                }
                else
                {
                    // Verificar si falta el rol "User" (por si ya existían otros roles)
                    if (!await context.Roles.AnyAsync(r => r.Name == "User"))
                    {
                        logger.LogInformation("Agregando rol User faltante...");
                        
                        var userRole = new Role
                        {
                            Name = "User",
                            Description = "Usuario registrado - Acceso gratuito limitado",
                            IsActive = true,
                            CreatedBy = "system",
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        await context.Roles.AddAsync(userRole);
                        await context.SaveChangesAsync();
                        logger.LogInformation("Rol User agregado correctamente");
                    }
                }

                // 2. Creacion de usuario Admin por defecto (si no existe)
                if (!await context.Users.AnyAsync(u => u.Email == "admin@gmail.com"))
                {
                    logger.LogInformation("Creando administrador...");

                    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@justina.com";
                    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Admin123!";

                    var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

                    if (adminRole != null)
                    {
                        var adminUser = new User
                        {
                            Email = adminEmail,
                            FirstName = "Admin",
                            LastName = "Sistema",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                            IsActive = true,
                            EmailConfirmed = true,
                            CreatedBy = "system",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            UserRoles = new List<UserRole>()
                        };

                        adminUser.UserRoles.Add(new UserRole
                        {
                            RoleId = adminRole.Id,
                            AssignedAt = DateTime.UtcNow,
                            AssignedBy = "system",
                        });

                        await context.Users.AddAsync(adminUser);
                        await context.SaveChangesAsync();

                        logger.LogInformation($"✅ Admin creado: {adminEmail}");
                    }
                }
                else
                {
                    logger.LogInformation("✅ Admin ya existe");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al inicializar la base de datos");
            }
        }
    }
}
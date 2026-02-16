using Application.Extensions;
using Infraestruture.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------
            // SERVICES
            // -------------------------

            builder.Services.AddControllers();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Autenticación
            ConfigureAuthentication(builder);

            // Swagger
            ConfigureSwagger(builder);

            var app = builder.Build();

            // Inicializar base de datos (Seeder)
            await InitializeDatabase(app);


            // -------------------------
            // PIPELINE
            // -------------------------

            ConfigureErrorHandler(app);

            app.UseCors("FrontendPolicy");

            ConfigurePipeline(app);

            await InitializeDatabase(app);

            app.Run();
        }

        private static void ConfigureErrorHandler(WebApplication app)
        {
            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                var statusCode = response.StatusCode;

                if (statusCode == 401 || statusCode == 403 || statusCode == 404 || statusCode == 500)
                {
                    response.ContentType = "application/json";

                    var errorResponse = new
                    {
                        StatusCode = statusCode,
                        Message = GetErrorMessage(statusCode),
                        Timestamp = DateTime.UtcNow
                    };

                    await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                }
            });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature?.Error;

                    var errorResponse = new
                    {
                        StatusCode = 500,
                        Message = "Error interno del servidor. Por favor, intente más tarde.",
                        Detail = app.Environment.IsDevelopment() ? exception?.Message : null,
                        Timestamp = DateTime.UtcNow
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                });
            });
        }

        private static string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                401 => "No autorizado. Token no proporcionado o inválido.",
                403 => "Acceso denegado. No tiene permisos para este recurso.",
                404 => "Recurso no encontrado.",
                500 => "Error interno del servidor.",
                _ => "Error inesperado."
            };
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var tokenKey = builder.Configuration["AppSettings:Token"]
                ?? throw new InvalidOperationException("Token no configurado en AppSettings");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var errorResponse = new
                            {
                                StatusCode = 401,
                                Message = "No autorizado. Token inválido o expirado.",
                                Timestamp = DateTime.UtcNow
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";

                            var errorResponse = new
                            {
                                StatusCode = 403,
                                Message = "Acceso denegado. Se requiere rol de Administrador.",
                                Timestamp = DateTime.UtcNow
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                        }
                    };
                });
        }

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Justina API",
                    Version = "v1",
                    Description = "API para simulación quirúrgica Justina",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipo Justina",
                        Email = "equipo@justina.com"
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        private static async Task InitializeDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                await DatabaseInitializer.InitializeAsync(services);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error al ejecutar el Seeder");
            }
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Justina API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}

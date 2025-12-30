using Login.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;

namespace Login.Common.Extensions
{
    public static class BuilderExtensions
    {
        public static WebApplicationBuilder AddArchitectures(this WebApplicationBuilder builder)
        {
            // 1. Configuração do appsettings
            ConfigureAppSettings(builder);

            // 2. Configuração OpenAPI com segurança JWT
            ConfigureOpenApi(builder);

            // 3. Configuração do DbContext
            ConfigureDbContext(builder);

            return builder;
        }

        private static void ConfigureOpenApi(WebApplicationBuilder builder)
        {
            // OpenAPI básico - segurança será inferida dos atributos [Authorize]
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
    
            // Para a UI do Swagger ainda mostrar o botão Authorize, podemos usar esta abordagem:
            builder.Services.Configure<OpenApiOptions>(options =>
            {
                // No .NET 10, a segurança é inferida automaticamente dos endpoints
                // que usam [Authorize] ou RequireAuthorization()
            });
        }
        private static void ConfigureDbContext(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlOptions => mysqlOptions.EnableRetryOnFailure()
                );
            });
        }

        private static void ConfigureAppSettings(WebApplicationBuilder builder)
        {
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
        }

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            // Adicione outros serviços aqui
            return builder;
        }

        public static WebApplicationBuilder AddToken(this WebApplicationBuilder builder)
        {
            // VALIDAR configurações antes de usar
            ValidateJwtConfiguration(builder.Configuration);

            var secretKey = builder.Configuration["TokenJWT:Secret"];
            var issuer = builder.Configuration["TokenJWT:Issuer"];
            var audience = builder.Configuration["TokenJWT:Audience"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret key is not configured. Check appsettings.json");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Mude para true em produção
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();
            return builder;
        }

        private static void ValidateJwtConfiguration(IConfiguration configuration)
        {
            var missingSettings = new List<string>();

            if (string.IsNullOrEmpty(configuration["TokenJWT:Secret"]))
                missingSettings.Add("TokenJWT:Secret");

            if (string.IsNullOrEmpty(configuration["TokenJWT:Issuer"]))
                missingSettings.Add("TokenJWT:Issuer");

            if (string.IsNullOrEmpty(configuration["TokenJWT:Audience"]))
                missingSettings.Add("TokenJWT:Audience");

            if (missingSettings.Any())
            {
                throw new InvalidOperationException(
                    $"Missing JWT configuration in appsettings.json: {string.Join(", ", missingSettings)}");
            }
        }

        public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, loggerConfig) =>
            {
                loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .WriteTo.File(
                        path: "logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    );
            });

            return builder;
        }

        public static WebApplication UseArchitectures(this WebApplication app)
        {
            // SEMPRE mapear OpenAPI (não apenas em Development)
            app.MapOpenApi();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "API v1");
                    options.DocumentTitle = "Minha API - Documentação";
                    options.RoutePrefix = "swagger";
                    options.DefaultModelsExpandDepth(-1); // Esconde schemas
                });

                app.MapGet("/", () => Results.Redirect("/swagger"));
            }

            return app;
        }





    }
}
using Application;
using Infrastructure;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.OpenApi.Models;

namespace AuthService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddOpenApi();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            ConfigureSwaggerWithJwt(builder.Services);
            ConfigureCors(builder.Services);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            builder.Logging.ClearProviders();
            builder.Logging.AddEventLog(settings =>
            {
                settings.SourceName = "AuthService";
                settings.LogName = "AuthService";
            });
            builder.Logging.AddFilter<EventLogLoggerProvider>("", LogLevel.Information);
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("AuthService started and logging is configured.");

            app.Run();
        }

        /// <summary>
        /// Configures Swagger with JWT support.
        /// </summary>
        private static void ConfigureSwaggerWithJwt(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AuthService API",
                    Version = "v1",
                    Description = "JWT Authentication and Authorization Service"
                });

                //Correct HTTP Bearer scheme
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter 'Bearer' [space] and then your token.\nExample: Bearer abc123",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // lowercase is important for Swagger UI
                    BearerFormat = "JWT"
                });

                //Apply Bearer token globally to all endpoints
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "bearer",
                            Name = "Authorization",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });

            });
        }

        /// <summary>
        /// Configures CORS policy.
        /// </summary>
        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }
    }
}

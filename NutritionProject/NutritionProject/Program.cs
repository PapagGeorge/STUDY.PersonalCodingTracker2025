using Application.Interfaces;
using Application.NutritionService;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NutritionProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHttpClient("ExternalApiClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<IApiHttpClient, ApiHttpClient>();
            builder.Services.AddTransient<INutritionixClient, NutritionixClient>();
            builder.Services.AddTransient<INutritionService, NutritionService>();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;

                        int statusCode = ex switch
                        {
                            ExternalApiException => 502,         // Bad Gateway for API errors
                            ArgumentException => 400,             // Bad Request for invalid input
                            UnauthorizedAccessException => 401,  // Unauthorized
                            KeyNotFoundException => 404,         // Not Found
                            _ => 500                             // Internal Server Error for others
                        };

                        var path = context.Request.Path;
                        logger.LogError(ex, "Error occurred processing request at {Path}", path);

                        var response = new
                        {                     
                            message = statusCode == 500
                                ? "An unexpected error occurred."
                                : ex.Message,
                            #if DEBUG
                            detail = ex.StackTrace
                            #endif

                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

using Application;
using Application.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<RecipeDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
                    
        // Register repositories
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<INutritionData, NutritionData>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
            client.BaseAddress = new Uri(settings.NutritionServiceBaseUrl);
        });
        services.AddScoped<INutritionService, NutritionService>();


        // Register domain services
        services.AddScoped<RecipeScalerService>();
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

        return services;
    }
}
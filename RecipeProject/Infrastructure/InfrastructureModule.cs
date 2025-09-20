using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces;
using Application;
using Infrastructure.Repositories;

namespace Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<RecipeDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));
                    
        // Register repositories
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<INutritionData, NutritionData>(client =>
        {
            client.BaseAddress = new Uri(configuration["NutritionService:BaseUrl"]);
        });
            
        // Register domain services
        services.AddScoped<RecipeScalerService>();
        
        return services;
    }
}
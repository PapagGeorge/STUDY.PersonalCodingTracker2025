using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces;

namespace Application;

public static class ApplicationModule
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<INutritionService, NutritionService>();

        return services;
    }
}
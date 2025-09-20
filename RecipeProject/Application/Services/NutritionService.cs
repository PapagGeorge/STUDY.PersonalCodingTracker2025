using System.Text.Json;
using Application.Interfaces;
using Infrastructure.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application;

public class NutritionService : INutritionService
{
    private readonly ILogger<NutritionService> _logger;
    
    private readonly INutritionData _nutritionData;

    public NutritionService(HttpClient httpClient,
        ILogger<NutritionService> logger,
        INutritionData nutritionData)
    {
        _logger = logger;
        _nutritionData = nutritionData;
    }

    public async Task<NutritionResponseDto> GetNutritionDataAsync(RecipeDto recipe)
    {
        try
        {
            var ingredientsQuery = string.Join(", ", recipe.Ingredients.Select(i =>
                $"{i.Amount} {i.Unit} {i.Name}"));

            _logger.LogInformation("Getting nutrition data for recipe: {RecipeName} with ingredients: {Ingredients}", 
                recipe.Title, ingredientsQuery);
            
            var result = await _nutritionData.GetNutritionDataAsync(ingredientsQuery);

            if (result == null || !result.Foods.Any())
            {
                _logger.LogWarning("No nutrition data found for recipe: {RecipeName}", recipe.Title);
                return new NutritionResponseDto { Foods = new List<FoodAttributeDto>() };
            }

            _logger.LogInformation("Successfully retrieved nutrition data for recipe: {RecipeName}, found {FoodCount} foods", 
                recipe.Title, result.Foods.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nutrition data for recipe: {RecipeName}", recipe.Title);
            throw;
        }
    }
}
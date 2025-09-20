using Infrastructure.DTOs;

namespace Application.Interfaces;

public interface INutritionService
{
    Task<NutritionResponseDto> GetNutritionDataAsync(RecipeDto recipe);
}
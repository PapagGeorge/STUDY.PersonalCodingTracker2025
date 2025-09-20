using Infrastructure.DTOs;

namespace Application.Interfaces;

public interface IRecipeService
{
    Task<RecipeDto?> GetRecipeByIdAsync(Guid id);
    Task<RecipeDto?> GetScaledRecipeAsync(Guid id, int servings);
    Task<IEnumerable<RecipeDto>> SearchRecipesAsync(string searchTerm);
    Task<Guid> CreateRecipeAsync(RecipeDto recipeDto);
    Task UpdateRecipeAsync(RecipeDto recipeDto);
    Task DeleteRecipeAsync(Guid id);
    Task<NutritionResponseDto> GetNutritionDataAsync(RecipeDto recipe);
}
using Infrastructure.DTOs;

namespace Application.Interfaces;

public interface INutritionData
{
    Task<NutritionResponseDto> GetNutritionDataAsync(string query);
}
using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;

namespace Application.Interfaces
{
    public interface INutritionService
    {
        Task<GetNutritionDataResponseDto> GetNutritionDataAsync(GetNutritionDataRequest request);
    }
}

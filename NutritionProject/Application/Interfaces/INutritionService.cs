using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;

namespace Application.Interfaces
{
    public interface INutritionService
    {
        Task<GetNutritionDataResponse> GetNutritionDataAsync(GetNutritionDataRequest request);
    }
}

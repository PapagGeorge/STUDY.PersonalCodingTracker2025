using Domain.GetNutritionData;

namespace Application.Interfaces
{
    public interface INutritionixClient
    {
        Task<GetNutritionDataResponse> GetNutritionDataAsync(GetNutritionDataRequest query);
    }
}

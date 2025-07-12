using Application.Interfaces;
using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;

namespace Application.NutritionService
{
    public class NutritionService : INutritionService
    {
        private readonly INutritionixClient _nutritionixClient;

        public NutritionService(INutritionixClient nutritionixClient)
        {
            _nutritionixClient = nutritionixClient;
        }

        public async Task<GetNutritionDataResponse> GetNutritionDataAsync(GetNutritionDataRequest request)
        {
            var response =  await _nutritionixClient.GetNutritionDataAsync(request);
            return response;
        }
    }
}

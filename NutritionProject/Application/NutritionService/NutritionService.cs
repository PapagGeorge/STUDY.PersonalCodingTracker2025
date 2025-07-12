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

        public async Task<GetNutritionDataResponseDto> GetNutritionDataAsync(GetNutritionDataRequest request)
        {
            var nutritionixResponse =  await _nutritionixClient.GetNutritionDataAsync(request);
            var response = nutritionixResponse.ToNutritionResponse();
            return response;
        }
    }
}
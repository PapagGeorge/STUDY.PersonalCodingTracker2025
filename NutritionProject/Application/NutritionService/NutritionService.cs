using Application.Interfaces;
using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;
using Microsoft.Extensions.Logging;

namespace Application.NutritionService
{
    public class NutritionService : INutritionService
    {
        private readonly INutritionixClient _nutritionixClient;
        private readonly ILogger<NutritionService> _logger;

        public NutritionService(INutritionixClient nutritionixClient, ILogger<NutritionService> logger)
        {
            _nutritionixClient = nutritionixClient;
            _logger = logger;
        }

        public async Task<GetNutritionDataResponseDto> GetNutritionDataAsync(GetNutritionDataRequest request)
        {
            var nutritionixResponse = await _nutritionixClient.GetNutritionDataAsync(request);
            var response =  nutritionixResponse.ToNutritionResponse();
            _logger.LogInformation("Response from Nutritionix Api successfully mapped");
            return response;
        }
    }

}
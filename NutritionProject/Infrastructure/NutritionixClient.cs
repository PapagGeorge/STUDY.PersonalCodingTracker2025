using Application.Interfaces;
using Domain.GetNutritionData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class NutritionixClient : INutritionixClient
    {
        private readonly IApiHttpClient _apiHttpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NutritionixClient> _logger;

        public NutritionixClient(IApiHttpClient apiHttpClient, IConfiguration configuration, ILogger<NutritionixClient> logger)
        {
            _apiHttpClient = apiHttpClient;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<GetNutritionDataResponse> GetNutritionDataAsync(GetNutritionDataRequest request)
        {
            try
            {
                var url = _configuration["Nutritionix:ApiBaseUrl"]
                          ?? throw new InvalidOperationException("Missing Nutritionix base URL");

                var headers = new Dictionary<string, string>
                {             
                    { "x-app-id", _configuration["Nutritionix:AppId"] ?? throw new InvalidOperationException() },
                    { "x-app-key", _configuration["Nutritionix:AppKey"] ?? throw new InvalidOperationException() }
                };

                return await _apiHttpClient.PostAsync<GetNutritionDataRequest, GetNutritionDataResponse>(url, request, headers);
            }
            catch (ExternalApiException ex)
            {
                _logger.LogError(ex, "Nutritionix API call failed.");
                throw;
            }
        }
    }
}
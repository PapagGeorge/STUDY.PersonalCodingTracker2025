using Application.Interfaces;
using Domain.GetNutritionData;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class NutritionixClient : INutritionixClient
    {
        private readonly IApiHttpClient _apiHttpClient;
        private readonly IConfiguration _configuration;

        public NutritionixClient(IApiHttpClient apiHttpClient, IConfiguration configuration)
        {
            _apiHttpClient = apiHttpClient;
            _configuration = configuration;
        }
        public async Task<GetNutritionDataResponse> GetNutritionDataAsync(GetNutritionDataRequest request)
        {
            var url = _configuration["Nutritionix:ApiBaseUrl"] ?? throw new InvalidOperationException();

            var headers = new Dictionary<string, string>
        {
            { "x-app-id", _configuration["Nutritionix:AppId"] ?? throw new InvalidOperationException() },
            { "x-app-key", _configuration["Nutritionix:AppKey"] ?? throw new InvalidOperationException() }
        };

            return await _apiHttpClient.PostAsync<GetNutritionDataRequest, GetNutritionDataResponse>(url, request, headers);
        }
    }
}
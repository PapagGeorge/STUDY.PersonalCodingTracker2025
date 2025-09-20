using System.Text.Json;
using Application.Interfaces;
using Infrastructure.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class NutritionData : INutritionData
{
    private readonly ILogger<NutritionData> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public NutritionData(HttpClient httpClient,
        ILogger<NutritionData> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    public async Task<NutritionResponseDto> GetNutritionDataAsync(string query)
    {
        try
        {
            var url = $"Nutrition/getnutritionData?Query={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Nutrition service returned non-success status: {StatusCode}", response.StatusCode);
                return new NutritionResponseDto { Foods = new List<FoodAttributeDto>() };
            }

            var stream = await response.Content.ReadAsStreamAsync();

            var nutritionData = await JsonSerializer.DeserializeAsync<NutritionResponseDto>(stream, _jsonOptions);

            return nutritionData ?? new NutritionResponseDto { Foods = new List<FoodAttributeDto>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call Nutrition API");
            return new NutritionResponseDto { Foods = new List<FoodAttributeDto>() };
        }
    }
}
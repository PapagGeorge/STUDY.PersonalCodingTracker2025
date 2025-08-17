using System.Text.Json.Serialization;

namespace Domain.GetNutritionDataDTOs
{
    public class GetNutritionDataResponseDto
    {
        [JsonPropertyName("foods")]
        public List<FoodDto> Foods { get; set; }
    }
}

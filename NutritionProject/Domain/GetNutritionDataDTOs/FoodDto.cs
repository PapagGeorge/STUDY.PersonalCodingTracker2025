using System.Text.Json.Serialization;

namespace Domain.GetNutritionDataDTOs
{
    public class FoodDto
    {
        [JsonPropertyName("foodName")]
        public string FoodName { get; set; } = string.Empty;

        [JsonPropertyName("calories")]
        public double? Calories { get; set; }

        [JsonPropertyName("carbohydrates")]
        public double? Carbohydrates { get; set; }

        [JsonPropertyName("protein")]
        public double? Protein { get; set; }

        [JsonPropertyName("totalFat")]
        public double? TotalFat { get; set; }

        [JsonPropertyName("servingDescription")]
        public string ServingDescription { get; set; } = string.Empty;

        [JsonPropertyName("full_nutrients")]
        public List<FullNutrientDto> FullNutrients { get; set; } = string.Empty;
    }
}

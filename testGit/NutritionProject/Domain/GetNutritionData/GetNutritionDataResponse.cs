using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class GetNutritionDataResponse
    {
        [JsonPropertyName("foods")]
        public List<Food> Foods { get; set; }
    }
}

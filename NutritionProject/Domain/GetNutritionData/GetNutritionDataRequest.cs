using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class GetNutritionDataRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }
    }
}

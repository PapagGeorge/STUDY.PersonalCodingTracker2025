using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class Metadata
    {
        [JsonPropertyName("is_raw_food")]
        public bool? IsRawFood { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Domain.GetNutritionDataDTOs
{
    public class FullNutrientDto
    {
        [JsonPropertyName("attr_id")]
        public int? AttrId { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }
    }
}

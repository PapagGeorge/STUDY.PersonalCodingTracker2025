using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class FullNutrient
    {
        [JsonPropertyName("attr_id")]
        public int? AttrId { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }
    }
}

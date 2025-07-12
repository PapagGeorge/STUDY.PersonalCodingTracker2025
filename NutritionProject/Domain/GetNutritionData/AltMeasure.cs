using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class AltMeasure
    {
        [JsonPropertyName("servingWeight")]
        public double? ServingWeight { get; set; }

        [JsonPropertyName("measure")]
        public string Measure { get; set; }

        [JsonPropertyName("seq")]
        public int? Seq { get; set; }

        [JsonPropertyName("qty")]
        public int? Qty { get; set; }
    }
}

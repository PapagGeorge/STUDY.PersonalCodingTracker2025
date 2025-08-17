using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class Tags
    {
        [JsonPropertyName("item")]
        public string Item { get; set; }

        [JsonPropertyName("measure")]
        public string Measure { get; set; }

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }

        [JsonPropertyName("food_group")]
        public double? FoodGroup { get; set; }

        [JsonPropertyName("tag_id")]
        public double? TagId { get; set; }
    }
}

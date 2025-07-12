using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class Photo
    {
        [JsonPropertyName("thumb")]
        public string Thumb { get; set; }

        [JsonPropertyName("highres")]
        public string Highres { get; set; }

        [JsonPropertyName("is_user_uploaded")]
        public bool? IsUserUploaded { get; set; }
    }
}

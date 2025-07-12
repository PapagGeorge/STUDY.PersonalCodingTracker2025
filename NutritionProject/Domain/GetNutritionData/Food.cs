using System.Text.Json.Serialization;

namespace Domain.GetNutritionData
{
    public class Food
    {
        [JsonPropertyName("food_name")]
        public string FoodName { get; set; }

        [JsonPropertyName("brand_name")]
        public string BrandName { get; set; }

        [JsonPropertyName("serving_qty")]
        public double? ServingQty { get; set; }

        [JsonPropertyName("serving_unit")]
        public string ServingUnit { get; set; }

        [JsonPropertyName("serving_weight_grams")]
        public double? ServingWeightGrams { get; set; }

        [JsonPropertyName("nf_calories")]
        public double? NfCalories { get; set; }

        [JsonPropertyName("nf_total_fat")]
        public double? NfTotalFat { get; set; }

        [JsonPropertyName("nf_saturated_fat")]
        public double? NfSaturatedFat { get; set; }

        [JsonPropertyName("nf_cholesterol")]
        public double? NfCholesterol { get; set; }

        [JsonPropertyName("nf_sodium")]
        public double? NfSodium { get; set; }

        [JsonPropertyName("nf_total_carbohydrate")]
        public double? NfTotalCarbohydrate { get; set; }

        [JsonPropertyName("nf_dietary_fiber")]
        public double? NfDietaryFiber { get; set; }

        [JsonPropertyName("nf_sugars")]
        public double? NfSugars { get; set; }

        [JsonPropertyName("nf_protein")]
        public double? NfProtein { get; set; }

        [JsonPropertyName("nf_potassium")]
        public double? NfPotassium { get; set; }

        [JsonPropertyName("nf_p")]
        public double? NfPhosphorus { get; set; }

        [JsonPropertyName("full_nutrients")]
        public List<FullNutrient> FullNutrients { get; set; }

        [JsonPropertyName("nixBrandName")]
        public string NixBrandName { get; set; }

        [JsonPropertyName("nixBrandId")]
        public string NixBrandId { get; set; }

        [JsonPropertyName("nixItemName")]
        public string NixItemName { get; set; }

        [JsonPropertyName("nixItemId")]
        public string NixItemId { get; set; }

        [JsonPropertyName("upc")]
        public string Upc { get; set; }

        [JsonPropertyName("consumedAt")]
        public DateTime? ConsumedAt { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("source")]
        public double? Source { get; set; }

        [JsonPropertyName("ndbNo")]
        public double? NdbNo { get; set; }

        [JsonPropertyName("tags")]
        public Tags Tags { get; set; }

        [JsonPropertyName("alt_measures")]
        public List<AltMeasure> AltMeasures { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lng")]
        public double? Lng { get; set; }

        [JsonPropertyName("meal_type")]
        public double? MealType { get; set; }

        [JsonPropertyName("photo")]
        public Photo Photo { get; set; }
    }
}

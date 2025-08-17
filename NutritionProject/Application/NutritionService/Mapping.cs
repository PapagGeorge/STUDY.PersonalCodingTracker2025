using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;

namespace Application.NutritionService
{
    public static class Mapping
    {
        public static GetNutritionDataResponseDto ToNutritionResponse(this GetNutritionDataResponse r)
        {
            var result = new GetNutritionDataResponseDto();
            result.Foods = r.Foods.Select(x => x.ToNutritionResponse()).ToList();
            return result;
        }

        private static FoodDto ToNutritionResponse(this Food r)
        {
            var result = new FoodDto();
            result.FoodName = r.FoodName;
            result.Calories = r.NfCalories;
            result.Carbohydrates = r.NfTotalCarbohydrate;
            result.Protein = r.NfProtein;
            result.TotalFat = r.NfTotalFat;
            result.ServingDescription = $"{r.ServingQty} {r.ServingUnit}";
            return result;
        }
    }
}

using Application.Interfaces;
using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;
using Microsoft.AspNetCore.Http;    
using Microsoft.AspNetCore.Mvc;

namespace NutritionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionController : ControllerBase
    {
        private readonly INutritionService _nutritionService;

        public NutritionController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        [HttpGet("getnutritionData")]
        public async Task<ActionResult<GetNutritionDataResponseDto>> GetNutriotionData([FromQuery] GetNutritionDataRequest request)
        {
            var result = await _nutritionService.GetNutritionDataAsync(request);

                if(!result.Foods.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}

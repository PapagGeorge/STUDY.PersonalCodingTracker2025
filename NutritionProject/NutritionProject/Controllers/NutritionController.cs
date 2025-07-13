using Application.Interfaces;
using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;
using Infrastructure;
using Microsoft.AspNetCore.Http;    
using Microsoft.AspNetCore.Mvc;

namespace NutritionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionController : ControllerBase
    {
        private readonly INutritionService _nutritionService;
        private readonly ILogger<NutritionixClient> _logger;

        public NutritionController(INutritionService nutritionService, ILogger<NutritionixClient> logger)
        {
            _nutritionService = nutritionService;
            _logger = logger;
        }

        [HttpGet("getnutritionData")]
        public async Task<ActionResult<GetNutritionDataResponseDto>> GetNutriotionData([FromQuery] GetNutritionDataRequest request)
        {
            var result = await _nutritionService.GetNutritionDataAsync(request);

            if (!result.Foods.Any())
            {
                _logger.LogWarning("No nutrition data found for request {@Request}", request);
                return NotFound(new { message = "No nutrition data found." });
            }
            return Ok(result);
        }
    }
}

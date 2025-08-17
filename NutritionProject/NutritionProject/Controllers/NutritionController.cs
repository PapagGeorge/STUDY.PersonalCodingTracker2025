using Application.Interfaces;
using Domain.GetNutritionData;
using Domain.GetNutritionDataDTOs;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace NutritionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionController : ControllerBase
    {
        private readonly INutritionService _nutritionService;
        private readonly IApiPerformanceTracker _performanceTracker;
        private readonly ILogger<NutritionixClient> _logger;

        public NutritionController(INutritionService nutritionService, IApiPerformanceTracker performanceTracker, ILogger<NutritionixClient> logger)
        {
            _nutritionService = nutritionService;
            _performanceTracker = performanceTracker;
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

        [HttpGet("getPerformanceData")]
        public IActionResult GetPerformanceData()
        {
            var data = _performanceTracker.GetAllPerformanceData();
            return Ok(data);
        }
    }
}

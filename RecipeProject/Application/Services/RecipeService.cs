using Application.Interfaces;
using Infrastructure.DTOs;
using Domain;


namespace Application;

public class RecipeService : IRecipeService
    {
        private readonly Interfaces.IUnitOfWork _unitOfWork;
        private readonly RecipeScalerService _recipeScaler;
        private const string NutritionQueue = "nutrition_query_queue";
        private readonly INutritionService _nutritionService;
        
        public RecipeService(
            INutritionService nutritionService,
            Interfaces.IUnitOfWork unitOfWork,
            RecipeScalerService recipeScaler)
        {
            _nutritionService = nutritionService;
            _unitOfWork = unitOfWork;
            _recipeScaler = recipeScaler;
        }
        
        public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
        {
            var recipe = await _unitOfWork.Recipes.GetByIdAsync(id);
            return recipe != null ? MapToDto(recipe) : null;
        }
        
        public async Task<RecipeDto?> GetScaledRecipeAsync(int id, int servings)
        {
            var recipe = await _unitOfWork.Recipes.GetByIdAsync(id);
            if (recipe == null) return null;
            
            // Use the domain service to scale the recipe
            var scaledRecipe = _recipeScaler.ScaleRecipe(recipe, servings);
            return MapToDto(scaledRecipe);
        }
        
        public async Task<IEnumerable<RecipeDto>> SearchRecipesAsync(string searchTerm)
        {
            var recipes = await _unitOfWork.Recipes.SearchAsync(searchTerm);
            return recipes.Select(MapToDto);
        }
        
        public async Task<int> CreateRecipeAsync(RecipeDto recipeDto)
        {
            var recipe = MapToEntity(recipeDto);
            recipe.CreatedAt = DateTime.UtcNow;
            recipe.UpdatedAt = DateTime.UtcNow;
            
            var id = await _unitOfWork.Recipes.CreateAsync(recipe);
            await _unitOfWork.SaveChangesAsync();
            return id;
        }
        
        public async Task<NutritionResponseDto> GetNutritionDataAsync(RecipeDto recipe)
        {
            var result = await _nutritionService.GetNutritionDataAsync(recipe);

            if (result == null || !result.Foods.Any())
            {
                return new NutritionResponseDto { Foods = new List<FoodAttributeDto>() };
            }

            return result;
        }
        
        public async Task UpdateRecipeAsync(RecipeDto recipeDto)
        {
            var recipe = MapToEntity(recipeDto);
            recipe.UpdatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Recipes.UpdateAsync(recipe);
            await _unitOfWork.SaveChangesAsync();
        }
        
        public async Task DeleteRecipeAsync(int id)
        {
            await _unitOfWork.Recipes.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
        
        // Helper methods for mapping between entities and DTOs
        private RecipeDto MapToDto(Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Author = recipe.Author,
                Description = recipe.Description,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                Servings = recipe.Servings,
                ImageUrl = recipe.ImageUrl,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt,
                
                Ingredients = recipe.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Ingredient.Id,
                    Name = i.Ingredient.Name,
                    Amount = i.Amount,
                    Unit = i.Unit,
                    Notes = i.Notes
                }).ToList(),
                
                Steps = recipe.Steps
                    .OrderBy(s => s.OrderNumber)
                    .Select(s => new StepDto
                    {
                        OrderNumber = s.OrderNumber,
                        Instruction = s.Instruction
                    }).ToList(),
                
                Categories = recipe.Categories.Select(c => c.Category.Name).ToList(),
                Tags = recipe.Tags.Select(t => t.Tag.Name).ToList()
            };
        }
        
        private Recipe MapToEntity(RecipeDto dto)
        {
            return new Recipe
            {
                Title = dto.Title,
                Author = dto.Author,
                Description = dto.Description,
                PrepTimeMinutes = dto.PrepTimeMinutes,
                CookTimeMinutes = dto.CookTimeMinutes,
                Servings = dto.Servings,
                ImageUrl = dto.ImageUrl,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,

                Ingredients = dto.Ingredients.Select(i => new RecipeIngredient
                {
                    Ingredient = new Ingredient
                    {
                        Name = i.Name
                        // Optionally add other Ingredient fields if you have them (like Id, if you're not using auto-increment)
                    },
                    Amount = i.Amount,
                    Unit = i.Unit,
                    Notes = i.Notes ?? string.Empty
                }).ToList(),

                Steps = dto.Steps.Select(s => new Step
                {
                    OrderNumber = s.OrderNumber,
                    Instruction = s.Instruction
                }).ToList(),

                Categories = dto.Categories.Select(c => new RecipeCategory
                {
                    Category = new Category
                    {
                        Name = c
                    }
                }).ToList(),

                Tags = dto.Tags.Select(t => new RecipeTag
                {
                    Tag = new Tag
                    {
                        Name = t
                    }
                }).ToList()
            };
        }

    }
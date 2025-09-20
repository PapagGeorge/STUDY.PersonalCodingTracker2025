namespace Domain;

public class RecipeCategory
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
        
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
namespace MealCycle.Domain.Recipes;

public sealed class Recipe
{
    public Recipe(Guid id, string title, IReadOnlyList<RecipeIngredient> ingredients, IReadOnlyList<string> steps)
    {
        Id = id;
        Title = title;
        Ingredients = ingredients;
        Steps = steps;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public string Title { get; private set; }

    public IReadOnlyList<RecipeIngredient> Ingredients { get; private set; }

    public IReadOnlyList<string> Steps { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public void Update(string title, IReadOnlyList<RecipeIngredient> ingredients, IReadOnlyList<string> steps)
    {
        Title = title;
        Ingredients = ingredients;
        Steps = steps;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}

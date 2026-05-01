namespace MealCycle.Application.Features.Recipes;

public sealed record RecipeModel(
    Guid Id,
    string Title,
    IReadOnlyList<RecipeIngredientModel> Ingredients,
    IReadOnlyList<string> Steps,
    DateTimeOffset UpdatedAtUtc);

namespace MealCycle.Application.Features.Recipes;

public sealed record CreateRecipeRequest(
    string Title,
    IReadOnlyList<RecipeIngredientModel> Ingredients,
    IReadOnlyList<string> Steps);

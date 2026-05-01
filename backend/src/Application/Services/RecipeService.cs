using MealCycle.Application.Features.Recipes;
using MealCycle.Application.Interfaces;
using MealCycle.Domain.Recipes;

namespace MealCycle.Application.Services;

public sealed class RecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<IReadOnlyList<RecipeModel>> ListAsync(CancellationToken cancellationToken)
    {
        var recipes = await _recipeRepository.ListAsync(cancellationToken);
        return recipes.Select(ToModel).ToList();
    }

    public async Task<RecipeModel?> GetAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(recipeId, cancellationToken);
        return recipe is null ? null : ToModel(recipe);
    }

    public async Task<RecipeModel> CreateAsync(CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = new Recipe(
            Guid.NewGuid(),
            request.Title.Trim(),
            request.Ingredients.Select(x => new RecipeIngredient(x.Name.Trim(), x.Quantity.Trim())).ToList(),
            request.Steps.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList());

        var created = await _recipeRepository.UpsertAsync(recipe, cancellationToken);
        return ToModel(created);
    }

    private static RecipeModel ToModel(Recipe recipe)
    {
        var ingredients = recipe.Ingredients
            .Select(x => new RecipeIngredientModel(x.Name, x.Quantity))
            .ToList();

        return new RecipeModel(recipe.Id, recipe.Title, ingredients, recipe.Steps, recipe.UpdatedAtUtc);
    }
}

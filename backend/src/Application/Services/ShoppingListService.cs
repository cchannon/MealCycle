using MealCycle.Application.Features.ShoppingLists;
using MealCycle.Application.Interfaces;

namespace MealCycle.Application.Services;

public sealed class ShoppingListService
{
    private sealed class AggregatedIngredient
    {
        public HashSet<string> Quantities { get; } = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> SourceMeals { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IRecipeRepository _recipeRepository;

    public ShoppingListService(IMealPlanRepository mealPlanRepository, IRecipeRepository recipeRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<IReadOnlyList<ShoppingListItemModel>> PreviewAsync(ShoppingListPreviewRequest request, CancellationToken cancellationToken)
    {
        if (request.MealPlanItemIds.Count == 0)
        {
            return [];
        }

        var selectedIds = request.MealPlanItemIds.ToHashSet();
        var mealPlanItems = await _mealPlanRepository.ListAsync(cancellationToken);
        var selectedMeals = mealPlanItems.Where(item => selectedIds.Contains(item.Id)).ToList();

        if (selectedMeals.Count == 0)
        {
            return [];
        }

        var aggregatedByName = new Dictionary<string, AggregatedIngredient>(StringComparer.OrdinalIgnoreCase);

        foreach (var meal in selectedMeals)
        {
            var recipe = await _recipeRepository.GetAsync(meal.RecipeId, cancellationToken);
            if (recipe is null)
            {
                continue;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                var ingredientName = ingredient.Name.Trim();
                if (ingredientName.Length == 0)
                {
                    continue;
                }

                if (!aggregatedByName.TryGetValue(ingredientName, out var aggregatedIngredient))
                {
                    aggregatedIngredient = new AggregatedIngredient();
                    aggregatedByName[ingredientName] = aggregatedIngredient;
                }

                var quantity = ingredient.Quantity.Trim();
                if (quantity.Length > 0)
                {
                    aggregatedIngredient.Quantities.Add(quantity);
                }

                aggregatedIngredient.SourceMeals.Add(meal.Label);
            }
        }

        return aggregatedByName
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new ShoppingListItemModel(
                pair.Key,
                pair.Value.Quantities.Count == 0 ? "to taste" : string.Join(" + ", pair.Value.Quantities),
                pair.Value.SourceMeals.OrderBy(meal => meal, StringComparer.OrdinalIgnoreCase).ToList()))
            .ToList();
    }
}
using MealCycle.Application.Interfaces;
using MealCycle.Domain.MealPlans;
using MealCycle.Domain.Recipes;

namespace MealCycle.UnitTests.Fakes;

internal sealed class TestRecipeRepository : IRecipeRepository
{
    private readonly Dictionary<Guid, Recipe> _recipes;

    public TestRecipeRepository()
    {
        _recipes = SeedRecipes().ToDictionary(recipe => recipe.Id, recipe => recipe);
    }

    public Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Recipe>>(_recipes.Values.ToList());
    }

    public Task<Recipe?> GetAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _recipes.TryGetValue(recipeId, out var recipe);
        return Task.FromResult(recipe);
    }

    public Task<Recipe> UpsertAsync(Recipe recipe, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _recipes[recipe.Id] = recipe;
        return Task.FromResult(recipe);
    }

    private static IReadOnlyList<Recipe> SeedRecipes()
    {
        return
        [
            new Recipe(
                Guid.Parse("3e9e6ba9-7199-4f4d-a2f3-7388f699a531"),
                "Weeknight Chili",
                [
                    new RecipeIngredient("Ground turkey", "500g"),
                    new RecipeIngredient("Black beans", "2 cans"),
                    new RecipeIngredient("Crushed tomatoes", "1 can"),
                    new RecipeIngredient("Onion", "1 diced")
                ],
                [
                    "Brown turkey with diced onion.",
                    "Add beans and tomatoes.",
                    "Simmer for 20 minutes and season to taste."
                ]),
            new Recipe(
                Guid.Parse("80e3f6ef-f229-4e02-a2b1-f6be777ed6f4"),
                "Lemon Chickpea Pasta",
                [
                    new RecipeIngredient("Pasta", "400g"),
                    new RecipeIngredient("Chickpeas", "1 can"),
                    new RecipeIngredient("Lemon", "1 zested and juiced"),
                    new RecipeIngredient("Garlic", "2 cloves")
                ],
                [
                    "Cook pasta until al dente.",
                    "Saute garlic and chickpeas in olive oil.",
                    "Toss pasta with lemon juice, zest, and chickpeas."
                ]),
            new Recipe(
                Guid.Parse("e4b9ecce-af6e-435d-a89f-a703c09157b9"),
                "Crispy Tofu Rice Bowls",
                [
                    new RecipeIngredient("Firm tofu", "400g"),
                    new RecipeIngredient("Cooked rice", "3 cups"),
                    new RecipeIngredient("Broccoli", "1 head"),
                    new RecipeIngredient("Soy sauce", "3 tbsp")
                ],
                [
                    "Press and cube tofu, then pan-fry until crisp.",
                    "Steam broccoli until bright green.",
                    "Serve tofu and broccoli over rice with soy sauce."
                ]),
            new Recipe(
                Guid.Parse("4cbac8df-cce0-4d8f-9072-681f4e6fbe59"),
                "Baked Salmon and Potatoes",
                [
                    new RecipeIngredient("Salmon fillets", "4"),
                    new RecipeIngredient("Baby potatoes", "700g"),
                    new RecipeIngredient("Olive oil", "2 tbsp"),
                    new RecipeIngredient("Dill", "1 tbsp")
                ],
                [
                    "Roast potatoes until nearly tender.",
                    "Season salmon and place on tray with potatoes.",
                    "Bake until salmon flakes and finish with dill."
                ])
        ];
    }
}

internal sealed class TestMealPlanRepository : IMealPlanRepository
{
    private readonly Dictionary<Guid, MealPlanItem> _items;

    public TestMealPlanRepository()
    {
        _items = SeedMealPlanItems().ToDictionary(item => item.Id, item => item);
    }

    public Task<IReadOnlyList<MealPlanItem>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<MealPlanItem>>(_items.Values.ToList());
    }

    public Task<MealPlanItem> UpsertAsync(MealPlanItem mealPlanItem, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items[mealPlanItem.Id] = mealPlanItem;
        return Task.FromResult(mealPlanItem);
    }

    public Task UpsertManyAsync(IReadOnlyList<MealPlanItem> mealPlanItems, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var mealPlanItem in mealPlanItems)
        {
            _items[mealPlanItem.Id] = mealPlanItem;
        }

        return Task.CompletedTask;
    }

    private static IReadOnlyList<MealPlanItem> SeedMealPlanItems()
    {
        return
        [
            new MealPlanItem(
                Guid.Parse("2de8e48e-8df2-4d28-abf7-bd765b2c4b2d"),
                Guid.Parse("3e9e6ba9-7199-4f4d-a2f3-7388f699a531"),
                "Monday",
                "Weeknight Chili",
                0),
            new MealPlanItem(
                Guid.Parse("f1783191-cba6-4f3f-aec7-d5fd5d4dc2cc"),
                Guid.Parse("80e3f6ef-f229-4e02-a2b1-f6be777ed6f4"),
                "Monday",
                "Lemon Chickpea Pasta",
                1),
            new MealPlanItem(
                Guid.Parse("73f6824d-b2b6-4bc0-b16c-765cdf3e0706"),
                Guid.Parse("e4b9ecce-af6e-435d-a89f-a703c09157b9"),
                "Tuesday",
                "Crispy Tofu Rice Bowls",
                0),
            new MealPlanItem(
                Guid.Parse("0a61b1f9-0ea1-45ca-bfaa-f122cfc08fce"),
                Guid.Parse("4cbac8df-cce0-4d8f-9072-681f4e6fbe59"),
                "Wednesday",
                "Baked Salmon and Potatoes",
                0)
        ];
    }
}

internal sealed class TestCookProgressRepository : ICookProgressRepository
{
    private readonly Dictionary<Guid, HashSet<int>> _completedStepIndexesByMeal = new();

    public Task<IReadOnlyCollection<int>> GetCompletedStepIndexesAsync(Guid mealPlanItemId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_completedStepIndexesByMeal.TryGetValue(mealPlanItemId, out var indexes))
        {
            return Task.FromResult<IReadOnlyCollection<int>>([]);
        }

        return Task.FromResult<IReadOnlyCollection<int>>(indexes.OrderBy(index => index).ToList());
    }

    public Task SetStepCompletionAsync(Guid mealPlanItemId, int stepIndex, bool isCompleted, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_completedStepIndexesByMeal.TryGetValue(mealPlanItemId, out var indexes))
        {
            indexes = [];
            _completedStepIndexesByMeal[mealPlanItemId] = indexes;
        }

        if (isCompleted)
        {
            indexes.Add(stepIndex);
        }
        else
        {
            indexes.Remove(stepIndex);
        }

        return Task.CompletedTask;
    }
}

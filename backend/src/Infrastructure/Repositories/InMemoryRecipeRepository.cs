using System.Collections.Concurrent;
using MealCycle.Application.Interfaces;
using MealCycle.Domain.Recipes;

namespace MealCycle.Infrastructure.Repositories;

public sealed class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly ConcurrentDictionary<Guid, Recipe> _recipes = new();

    public InMemoryRecipeRepository()
    {
        var seededRecipes = new[]
        {
            new Recipe(
                Guid.Parse("3e9e6ba9-7199-4f4d-a2f3-7388f699a531"),
                "Weeknight Chili",
                new List<RecipeIngredient>
                {
                    new("Ground turkey", "500g"),
                    new("Black beans", "2 cans"),
                    new("Crushed tomatoes", "1 can"),
                    new("Onion", "1 diced")
                },
                new List<string>
                {
                    "Brown turkey with diced onion.",
                    "Add beans and tomatoes.",
                    "Simmer for 20 minutes and season to taste."
                }),
            new Recipe(
                Guid.Parse("80e3f6ef-f229-4e02-a2b1-f6be777ed6f4"),
                "Lemon Chickpea Pasta",
                new List<RecipeIngredient>
                {
                    new("Pasta", "400g"),
                    new("Chickpeas", "1 can"),
                    new("Lemon", "1 zested and juiced"),
                    new("Garlic", "2 cloves")
                },
                new List<string>
                {
                    "Cook pasta until al dente.",
                    "Saute garlic and chickpeas in olive oil.",
                    "Toss pasta with lemon juice, zest, and chickpeas."
                }),
            new Recipe(
                Guid.Parse("e4b9ecce-af6e-435d-a89f-a703c09157b9"),
                "Crispy Tofu Rice Bowls",
                new List<RecipeIngredient>
                {
                    new("Firm tofu", "400g"),
                    new("Cooked rice", "3 cups"),
                    new("Broccoli", "1 head"),
                    new("Soy sauce", "3 tbsp")
                },
                new List<string>
                {
                    "Press and cube tofu, then pan-fry until crisp.",
                    "Steam broccoli until bright green.",
                    "Serve tofu and broccoli over rice with soy sauce."
                }),
            new Recipe(
                Guid.Parse("4cbac8df-cce0-4d8f-9072-681f4e6fbe59"),
                "Baked Salmon and Potatoes",
                new List<RecipeIngredient>
                {
                    new("Salmon fillets", "4"),
                    new("Baby potatoes", "700g"),
                    new("Olive oil", "2 tbsp"),
                    new("Dill", "1 tbsp")
                },
                new List<string>
                {
                    "Roast potatoes until nearly tender.",
                    "Season salmon and place on tray with potatoes.",
                    "Bake until salmon flakes and finish with dill."
                }),
        };

        foreach (var recipe in seededRecipes)
        {
            _recipes.TryAdd(recipe.Id, recipe);
        }
    }

    public Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var ordered = _recipes.Values
            .OrderByDescending(recipe => recipe.UpdatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<Recipe>>(ordered);
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
        _recipes.AddOrUpdate(recipe.Id, recipe, (_, _) => recipe);
        return Task.FromResult(recipe);
    }
}

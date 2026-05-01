using MealCycle.Application.Features.Recipes;
using MealCycle.Application.Services;
using MealCycle.UnitTests.Fakes;

namespace MealCycle.UnitTests;

public sealed class RecipeServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenValidRequest_AddsRecipe()
    {
        var repository = new TestRecipeRepository();
        var service = new RecipeService(repository);

        var request = new CreateRecipeRequest(
            "Tomato Soup",
            new List<RecipeIngredientModel>
            {
                new("Tomatoes", "6"),
                new("Vegetable stock", "500ml")
            },
            new List<string> { "Blend tomatoes.", "Simmer with stock." });

        var created = await service.CreateAsync(request, CancellationToken.None);
        var recipes = await service.ListAsync(CancellationToken.None);

        Assert.Equal("Tomato Soup", created.Title);
        Assert.Contains(recipes, recipe => recipe.Id == created.Id);
    }
}

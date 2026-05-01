using MealCycle.Application.Features.ShoppingLists;
using MealCycle.Application.Services;
using MealCycle.Infrastructure.Repositories;

namespace MealCycle.UnitTests;

public sealed class ShoppingListServiceTests
{
    [Fact]
    public async Task PreviewAsync_WhenMealsSelected_AggregatesIngredientsFromMatchingRecipes()
    {
        var mealPlanRepository = new InMemoryMealPlanRepository();
        var recipeRepository = new InMemoryRecipeRepository();
        var service = new ShoppingListService(mealPlanRepository, recipeRepository);

        var mealPlanItems = await mealPlanRepository.ListAsync(CancellationToken.None);
        var mondayChili = mealPlanItems.Single(item => item.Label == "Weeknight Chili");

        var preview = await service.PreviewAsync(
            new ShoppingListPreviewRequest([mondayChili.Id]),
            CancellationToken.None);

        Assert.NotEmpty(preview);
        Assert.Contains(preview, item => item.Name == "Black beans");
        Assert.Contains(preview, item => item.Name == "Crushed tomatoes");
        Assert.Contains(preview, item => item.Name == "Onion");
    }

    [Fact]
    public async Task PreviewAsync_WhenNoSelectedMeals_ReturnsEmptyList()
    {
        var mealPlanRepository = new InMemoryMealPlanRepository();
        var recipeRepository = new InMemoryRecipeRepository();
        var service = new ShoppingListService(mealPlanRepository, recipeRepository);

        var preview = await service.PreviewAsync(new ShoppingListPreviewRequest([]), CancellationToken.None);

        Assert.Empty(preview);
    }

    [Fact]
    public async Task PreviewAsync_WhenSelectedMealRecipeMissing_SkipsMissingRecipe()
    {
        var mealPlanRepository = new InMemoryMealPlanRepository();
        var recipeRepository = new InMemoryRecipeRepository();
        var service = new ShoppingListService(mealPlanRepository, recipeRepository);

        var missingRecipeMeal = await mealPlanRepository.UpsertAsync(
            new MealCycle.Domain.MealPlans.MealPlanItem(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                "Friday",
                "Missing Recipe Meal",
                0),
            CancellationToken.None);

        var preview = await service.PreviewAsync(
            new ShoppingListPreviewRequest([missingRecipeMeal.Id]),
            CancellationToken.None);

        Assert.Empty(preview);
    }
}
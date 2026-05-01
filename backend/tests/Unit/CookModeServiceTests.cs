using MealCycle.Application.Services;
using MealCycle.Infrastructure.Repositories;

namespace MealCycle.UnitTests;

public sealed class CookModeServiceTests
{
    [Fact]
    public async Task GetSessionAsync_WhenMealExists_ReturnsRecipeSteps()
    {
        var service = new CookModeService(
            new InMemoryMealPlanRepository(),
            new InMemoryRecipeRepository(),
            new InMemoryCookProgressRepository());

        var mealPlanItemId = Guid.Parse("2de8e48e-8df2-4d28-abf7-bd765b2c4b2d");
        var session = await service.GetSessionAsync(mealPlanItemId, CancellationToken.None);

        Assert.NotNull(session);
        Assert.Equal("Weeknight Chili", session.MealLabel);
        Assert.Equal(3, session.Steps.Count);
        Assert.All(session.Steps, step => Assert.False(step.IsCompleted));
    }

    [Fact]
    public async Task SetStepCompletionAsync_WhenStepMarkedCompleted_PersistsCompletionState()
    {
        var service = new CookModeService(
            new InMemoryMealPlanRepository(),
            new InMemoryRecipeRepository(),
            new InMemoryCookProgressRepository());

        var mealPlanItemId = Guid.Parse("2de8e48e-8df2-4d28-abf7-bd765b2c4b2d");
        var updated = await service.SetStepCompletionAsync(mealPlanItemId, 1, true, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.True(updated.Steps.Single(step => step.StepIndex == 1).IsCompleted);

        var reloaded = await service.GetSessionAsync(mealPlanItemId, CancellationToken.None);
        Assert.NotNull(reloaded);
        Assert.True(reloaded.Steps.Single(step => step.StepIndex == 1).IsCompleted);
    }

    [Fact]
    public async Task GetSessionAsync_WhenRecipeMissing_ReturnsSessionWithoutSteps()
    {
        var mealPlanRepository = new InMemoryMealPlanRepository();
        var missingRecipeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        await mealPlanRepository.UpsertAsync(
            new MealCycle.Domain.MealPlans.MealPlanItem(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                missingRecipeId,
                "Thursday",
                "Ghost Recipe",
                0),
            CancellationToken.None);

        var service = new CookModeService(
            mealPlanRepository,
            new InMemoryRecipeRepository(),
            new InMemoryCookProgressRepository());

        var session = await service.GetSessionAsync(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            CancellationToken.None);

        Assert.NotNull(session);
        Assert.Empty(session.Steps);
    }
}
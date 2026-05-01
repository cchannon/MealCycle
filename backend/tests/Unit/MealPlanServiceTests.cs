using MealCycle.Application.Features.MealPlans;
using MealCycle.Application.Services;
using MealCycle.UnitTests.Fakes;

namespace MealCycle.UnitTests;

public sealed class MealPlanServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenValidRequest_AddsMealToDay()
    {
        var repository = new TestMealPlanRepository();
        var service = new MealPlanService(repository, new TestRecipeRepository());

        var created = await service.CreateAsync(
            new CreateMealPlanItemRequest(
                "Thursday",
                Guid.Parse("3e9e6ba9-7199-4f4d-a2f3-7388f699a531")),
            CancellationToken.None);
        var mealPlan = await service.ListAsync(CancellationToken.None);

        Assert.NotNull(created);
        Assert.Equal("Thursday", created.Day);
        Assert.Equal("Weeknight Chili", created.Label);
        Assert.Contains(mealPlan, item => item.Id == created.Id);
    }

    [Fact]
    public async Task ReorderAsync_WhenItemsMovedAcrossDays_PersistsDayAndSortOrder()
    {
        var repository = new TestMealPlanRepository();
        var service = new MealPlanService(repository, new TestRecipeRepository());
        var initial = await service.ListAsync(CancellationToken.None);

        var moved = initial.First(item => item.Day == "Wednesday");
        var mondayFirst = initial.First(item => item.Day == "Monday" && item.SortOrder == 0);
        var mondaySecond = initial.First(item => item.Day == "Monday" && item.SortOrder == 1);

        var request = new ReorderMealPlanRequest(
            new List<MealPlanItemOrderModel>
            {
                new(moved.Id, "Monday", 0),
                new(mondayFirst.Id, "Monday", 1),
                new(mondaySecond.Id, "Monday", 2),
            });

        var updated = await service.ReorderAsync(request, CancellationToken.None);
        var mondayItems = updated.Where(item => item.Day == "Monday").OrderBy(item => item.SortOrder).ToList();

        Assert.Equal(3, mondayItems.Count);
        Assert.Equal(moved.Id, mondayItems[0].Id);
        Assert.Equal(mondayFirst.Id, mondayItems[1].Id);
        Assert.Equal(mondaySecond.Id, mondayItems[2].Id);
    }

    [Fact]
    public async Task CreateAsync_WhenRecipeDoesNotExist_ReturnsNull()
    {
        var repository = new TestMealPlanRepository();
        var service = new MealPlanService(repository, new TestRecipeRepository());

        var created = await service.CreateAsync(
            new CreateMealPlanItemRequest("Friday", Guid.NewGuid()),
            CancellationToken.None);

        Assert.Null(created);
    }
}
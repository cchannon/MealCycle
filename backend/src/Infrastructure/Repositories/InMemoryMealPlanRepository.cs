using System.Collections.Concurrent;
using MealCycle.Application.Interfaces;
using MealCycle.Domain.MealPlans;

namespace MealCycle.Infrastructure.Repositories;

public sealed class InMemoryMealPlanRepository : IMealPlanRepository
{
    private readonly ConcurrentDictionary<Guid, MealPlanItem> _items = new();

    public InMemoryMealPlanRepository()
    {
        var seedItems = new[]
        {
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
                0),
        };

        foreach (var item in seedItems)
        {
            _items.TryAdd(item.Id, item);
        }
    }

    public Task<IReadOnlyList<MealPlanItem>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<MealPlanItem>>(_items.Values.ToList());
    }

    public Task<MealPlanItem> UpsertAsync(MealPlanItem mealPlanItem, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.AddOrUpdate(mealPlanItem.Id, mealPlanItem, (_, _) => mealPlanItem);
        return Task.FromResult(mealPlanItem);
    }

    public Task UpsertManyAsync(IReadOnlyList<MealPlanItem> mealPlanItems, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var mealPlanItem in mealPlanItems)
        {
            _items.AddOrUpdate(mealPlanItem.Id, mealPlanItem, (_, _) => mealPlanItem);
        }

        return Task.CompletedTask;
    }
}
using MealCycle.Domain.MealPlans;

namespace MealCycle.Application.Interfaces;

public interface IMealPlanRepository
{
    Task<IReadOnlyList<MealPlanItem>> ListAsync(CancellationToken cancellationToken);

    Task<MealPlanItem> UpsertAsync(MealPlanItem mealPlanItem, CancellationToken cancellationToken);

    Task UpsertManyAsync(IReadOnlyList<MealPlanItem> mealPlanItems, CancellationToken cancellationToken);
}
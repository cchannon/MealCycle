namespace MealCycle.Application.Interfaces;

public interface ICookProgressRepository
{
    Task<IReadOnlyCollection<int>> GetCompletedStepIndexesAsync(Guid mealPlanItemId, CancellationToken cancellationToken);

    Task SetStepCompletionAsync(Guid mealPlanItemId, int stepIndex, bool isCompleted, CancellationToken cancellationToken);
}
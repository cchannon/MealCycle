using System.Collections.Concurrent;
using MealCycle.Application.Interfaces;

namespace MealCycle.Infrastructure.Repositories;

public sealed class InMemoryCookProgressRepository : ICookProgressRepository
{
    private readonly ConcurrentDictionary<Guid, HashSet<int>> _completedStepIndexesByMeal = new();
    private readonly object _syncLock = new();

    public Task<IReadOnlyCollection<int>> GetCompletedStepIndexesAsync(Guid mealPlanItemId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_completedStepIndexesByMeal.TryGetValue(mealPlanItemId, out var indexes))
        {
            return Task.FromResult<IReadOnlyCollection<int>>([]);
        }

        lock (_syncLock)
        {
            return Task.FromResult<IReadOnlyCollection<int>>(indexes.OrderBy(index => index).ToList());
        }
    }

    public Task SetStepCompletionAsync(Guid mealPlanItemId, int stepIndex, bool isCompleted, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var indexes = _completedStepIndexesByMeal.GetOrAdd(mealPlanItemId, _ => []);

        lock (_syncLock)
        {
            if (isCompleted)
            {
                indexes.Add(stepIndex);
            }
            else
            {
                indexes.Remove(stepIndex);
            }
        }

        return Task.CompletedTask;
    }
}
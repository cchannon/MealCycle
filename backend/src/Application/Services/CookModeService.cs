using MealCycle.Application.Features.CookMode;
using MealCycle.Application.Interfaces;

namespace MealCycle.Application.Services;

public sealed class CookModeService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICookProgressRepository _cookProgressRepository;

    public CookModeService(
        IMealPlanRepository mealPlanRepository,
        IRecipeRepository recipeRepository,
        ICookProgressRepository cookProgressRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _recipeRepository = recipeRepository;
        _cookProgressRepository = cookProgressRepository;
    }

    public async Task<CookModeSessionModel?> GetSessionAsync(Guid mealPlanItemId, CancellationToken cancellationToken)
    {
        var mealPlanItems = await _mealPlanRepository.ListAsync(cancellationToken);
        var mealPlanItem = mealPlanItems.FirstOrDefault(item => item.Id == mealPlanItemId);
        if (mealPlanItem is null)
        {
            return null;
        }

        var recipe = await _recipeRepository.GetAsync(mealPlanItem.RecipeId, cancellationToken);

        var completedStepIndexes = await _cookProgressRepository.GetCompletedStepIndexesAsync(mealPlanItemId, cancellationToken);
        var completedLookup = completedStepIndexes.ToHashSet();

        var steps = recipe?.Steps
            .Select((instruction, index) => new CookModeStepModel(index, instruction, completedLookup.Contains(index)))
            .ToList() ?? [];

        return new CookModeSessionModel(mealPlanItem.Id, mealPlanItem.Day, mealPlanItem.Label, steps);
    }

    public async Task<CookModeSessionModel?> SetStepCompletionAsync(
        Guid mealPlanItemId,
        int stepIndex,
        bool isCompleted,
        CancellationToken cancellationToken)
    {
        var session = await GetSessionAsync(mealPlanItemId, cancellationToken);
        if (session is null)
        {
            return null;
        }

        if (stepIndex < 0 || stepIndex >= session.Steps.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(stepIndex));
        }

        await _cookProgressRepository.SetStepCompletionAsync(mealPlanItemId, stepIndex, isCompleted, cancellationToken);
        return await GetSessionAsync(mealPlanItemId, cancellationToken);
    }
}
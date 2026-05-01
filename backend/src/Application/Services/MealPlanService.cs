using MealCycle.Application.Features.MealPlans;
using MealCycle.Application.Interfaces;
using MealCycle.Domain.MealPlans;

namespace MealCycle.Application.Services;

public sealed class MealPlanService
{
    private static readonly IReadOnlyDictionary<string, int> DayOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["Monday"] = 1,
        ["Tuesday"] = 2,
        ["Wednesday"] = 3,
        ["Thursday"] = 4,
        ["Friday"] = 5,
        ["Saturday"] = 6,
        ["Sunday"] = 7,
    };

    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IRecipeRepository _recipeRepository;

    public MealPlanService(IMealPlanRepository mealPlanRepository, IRecipeRepository recipeRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<IReadOnlyList<MealPlanItemModel>> ListAsync(CancellationToken cancellationToken)
    {
        var mealPlanItems = await _mealPlanRepository.ListAsync(cancellationToken);
        return mealPlanItems
            .OrderBy(item => DayOrder.TryGetValue(item.Day, out var dayOrder) ? dayOrder : int.MaxValue)
            .ThenBy(item => item.SortOrder)
            .ThenBy(item => item.Label, StringComparer.OrdinalIgnoreCase)
            .Select(ToModel)
            .ToList();
    }

    public async Task<MealPlanItemModel?> CreateAsync(CreateMealPlanItemRequest request, CancellationToken cancellationToken)
    {
        var day = NormalizeDay(request.Day);
        var recipe = await _recipeRepository.GetAsync(request.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return null;
        }

        var label = recipe.Title.Trim();

        var existing = await _mealPlanRepository.ListAsync(cancellationToken);
        var nextSortOrder = existing
            .Where(item => string.Equals(item.Day, day, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.SortOrder)
            .DefaultIfEmpty(-1)
            .Max() + 1;

        var created = await _mealPlanRepository.UpsertAsync(
            new MealPlanItem(Guid.NewGuid(), request.RecipeId, day, label, nextSortOrder),
            cancellationToken);

        return ToModel(created);
    }

    public async Task<IReadOnlyList<MealPlanItemModel>> ReorderAsync(ReorderMealPlanRequest request, CancellationToken cancellationToken)
    {
        var existing = await _mealPlanRepository.ListAsync(cancellationToken);
        var existingById = existing.ToDictionary(item => item.Id, item => item);

        foreach (var orderedItem in request.Items)
        {
            if (!existingById.TryGetValue(orderedItem.Id, out var mealPlanItem))
            {
                continue;
            }

            mealPlanItem.MoveTo(NormalizeDay(orderedItem.Day), orderedItem.SortOrder);
        }

        await _mealPlanRepository.UpsertManyAsync(existingById.Values.ToList(), cancellationToken);
        return await ListAsync(cancellationToken);
    }

    private static MealPlanItemModel ToModel(MealPlanItem mealPlanItem)
    {
        return new MealPlanItemModel(
            mealPlanItem.Id,
            mealPlanItem.RecipeId,
            mealPlanItem.Day,
            mealPlanItem.Label,
            mealPlanItem.SortOrder,
            mealPlanItem.UpdatedAtUtc);
    }

    private static string NormalizeDay(string day)
    {
        var trimmed = day.Trim();
        return trimmed.Length == 0
            ? trimmed
            : char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();
    }
}
namespace MealCycle.Application.Features.MealPlans;

public sealed record MealPlanItemModel(
    Guid Id,
    Guid RecipeId,
    string Day,
    string Label,
    int SortOrder,
    DateTimeOffset UpdatedAtUtc);
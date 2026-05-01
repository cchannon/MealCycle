namespace MealCycle.Application.Features.CookMode;

public sealed record CookModeSessionModel(
    Guid MealPlanItemId,
    string Day,
    string MealLabel,
    IReadOnlyList<CookModeStepModel> Steps);
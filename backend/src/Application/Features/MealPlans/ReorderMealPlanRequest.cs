namespace MealCycle.Application.Features.MealPlans;

public sealed record ReorderMealPlanRequest(IReadOnlyList<MealPlanItemOrderModel> Items);
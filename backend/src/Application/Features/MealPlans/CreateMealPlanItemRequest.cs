namespace MealCycle.Application.Features.MealPlans;

public sealed record CreateMealPlanItemRequest(string Day, Guid RecipeId);
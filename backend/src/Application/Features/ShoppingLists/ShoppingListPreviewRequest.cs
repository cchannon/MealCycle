namespace MealCycle.Application.Features.ShoppingLists;

public sealed record ShoppingListPreviewRequest(IReadOnlyList<Guid> MealPlanItemIds);
namespace MealCycle.Application.Features.ShoppingLists;

public sealed record ShoppingListItemModel(
    string Name,
    string Quantity,
    IReadOnlyList<string> SourceMeals);
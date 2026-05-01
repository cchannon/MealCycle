namespace MealCycle.Domain.MealPlans;

public sealed class MealPlanItem
{
    public MealPlanItem(
        Guid id,
        Guid recipeId,
        string day,
        string label,
        int sortOrder,
        DateTimeOffset? updatedAtUtc = null)
    {
        Id = id;
        RecipeId = recipeId;
        Day = day;
        Label = label;
        SortOrder = sortOrder;
        UpdatedAtUtc = updatedAtUtc ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public Guid RecipeId { get; }

    public string Day { get; private set; }

    public string Label { get; private set; }

    public int SortOrder { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public void MoveTo(string day, int sortOrder)
    {
        Day = day;
        SortOrder = sortOrder;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
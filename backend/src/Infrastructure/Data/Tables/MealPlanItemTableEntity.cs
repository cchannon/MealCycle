using Azure;
using Azure.Data.Tables;

namespace MealCycle.Infrastructure.Data.Tables;

internal sealed class MealPlanItemTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = TableEntityConstants.MealPlanPartitionKey;

    public string RowKey { get; set; } = string.Empty;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string RecipeId { get; set; } = string.Empty;

    public string Day { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}

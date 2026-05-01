using Azure;
using Azure.Data.Tables;

namespace MealCycle.Infrastructure.Data.Tables;

internal sealed class RecipeTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = TableEntityConstants.RecipePartitionKey;

    public string RowKey { get; set; } = string.Empty;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string Title { get; set; } = string.Empty;

    public string IngredientsJson { get; set; } = "[]";

    public string StepsJson { get; set; } = "[]";

    public DateTimeOffset UpdatedAtUtc { get; set; }
}

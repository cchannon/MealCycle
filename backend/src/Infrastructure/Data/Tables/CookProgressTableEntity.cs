using Azure;
using Azure.Data.Tables;

namespace MealCycle.Infrastructure.Data.Tables;

internal sealed class CookProgressTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = TableEntityConstants.CookProgressPartitionKey;

    public string RowKey { get; set; } = string.Empty;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string CompletedStepIndexesJson { get; set; } = "[]";
}

using System.Text.Json;
using Azure.Data.Tables;
using MealCycle.Application.Configuration;
using MealCycle.Application.Interfaces;
using MealCycle.Infrastructure.Data;
using MealCycle.Infrastructure.Data.Tables;
using Microsoft.Extensions.Options;

namespace MealCycle.Infrastructure.Repositories;

public sealed class AzureTableCookProgressRepository : ICookProgressRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly TableClient _tableClient;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public AzureTableCookProgressRepository(TableServiceClient tableServiceClient, IOptions<AzureStorageOptions> options)
    {
        var tableName = options.Value.CookProgressTableName;
        _tableClient = tableServiceClient.GetTableClient(tableName);
    }

    public async Task<IReadOnlyCollection<int>> GetCompletedStepIndexesAsync(Guid mealPlanItemId, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        var response = await _tableClient.GetEntityIfExistsAsync<CookProgressTableEntity>(
            TableEntityConstants.CookProgressPartitionKey,
            mealPlanItemId.ToString("N"),
            cancellationToken: cancellationToken);

        if (!response.HasValue)
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<int>>(response.Value!.CompletedStepIndexesJson, JsonOptions)
               ?.OrderBy(index => index)
               .ToList() ?? [];
    }

    public async Task SetStepCompletionAsync(Guid mealPlanItemId, int stepIndex, bool isCompleted, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        var response = await _tableClient.GetEntityIfExistsAsync<CookProgressTableEntity>(
            TableEntityConstants.CookProgressPartitionKey,
            mealPlanItemId.ToString("N"),
            cancellationToken: cancellationToken);

        var indexes = response.HasValue
            ? JsonSerializer.Deserialize<List<int>>(response.Value!.CompletedStepIndexesJson, JsonOptions) ?? []
            : [];

        if (isCompleted)
        {
            if (!indexes.Contains(stepIndex))
            {
                indexes.Add(stepIndex);
            }
        }
        else
        {
            indexes.Remove(stepIndex);
        }

        var entity = response.HasValue ? response.Value! : new CookProgressTableEntity
        {
            PartitionKey = TableEntityConstants.CookProgressPartitionKey,
            RowKey = mealPlanItemId.ToString("N"),
        };

        entity.CompletedStepIndexesJson = JsonSerializer.Serialize(indexes, JsonOptions);
        await _tableClient.UpsertEntityAsync(entity, cancellationToken: cancellationToken);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await _initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            await _tableClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }
}

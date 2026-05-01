using Azure.Data.Tables;
using MealCycle.Application.Configuration;
using MealCycle.Application.Interfaces;
using MealCycle.Domain.MealPlans;
using MealCycle.Infrastructure.Data;
using MealCycle.Infrastructure.Data.Tables;
using Microsoft.Extensions.Options;

namespace MealCycle.Infrastructure.Repositories;

public sealed class AzureTableMealPlanRepository : IMealPlanRepository
{
    private readonly TableClient _tableClient;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public AzureTableMealPlanRepository(TableServiceClient tableServiceClient, IOptions<AzureStorageOptions> options)
    {
        var tableName = options.Value.MealPlanTableName;
        _tableClient = tableServiceClient.GetTableClient(tableName);
    }

    public async Task<IReadOnlyList<MealPlanItem>> ListAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        var items = new List<MealPlanItem>();
        await foreach (var entity in _tableClient.QueryAsync<MealPlanItemTableEntity>(cancellationToken: cancellationToken))
        {
            items.Add(ToDomain(entity));
        }

        return items;
    }

    public async Task<MealPlanItem> UpsertAsync(MealPlanItem mealPlanItem, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _tableClient.UpsertEntityAsync(ToEntity(mealPlanItem), cancellationToken: cancellationToken);
        return mealPlanItem;
    }

    public async Task UpsertManyAsync(IReadOnlyList<MealPlanItem> mealPlanItems, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        foreach (var mealPlanItem in mealPlanItems)
        {
            await _tableClient.UpsertEntityAsync(ToEntity(mealPlanItem), cancellationToken: cancellationToken);
        }
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

            var hasRows = false;
            await foreach (var _ in _tableClient.QueryAsync<MealPlanItemTableEntity>(maxPerPage: 1, cancellationToken: cancellationToken))
            {
                hasRows = true;
                break;
            }

            if (!hasRows)
            {
                foreach (var seedItem in SeedData.MealPlanItems)
                {
                    await _tableClient.UpsertEntityAsync(ToEntity(seedItem), cancellationToken: cancellationToken);
                }
            }

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private static MealPlanItemTableEntity ToEntity(MealPlanItem item)
    {
        return new MealPlanItemTableEntity
        {
            PartitionKey = TableEntityConstants.MealPlanPartitionKey,
            RowKey = item.Id.ToString("N"),
            RecipeId = item.RecipeId.ToString("N"),
            Day = item.Day,
            Label = item.Label,
            SortOrder = item.SortOrder,
            UpdatedAtUtc = item.UpdatedAtUtc,
        };
    }

    private static MealPlanItem ToDomain(MealPlanItemTableEntity entity)
    {
        return new MealPlanItem(
            Guid.Parse(entity.RowKey),
            Guid.Parse(entity.RecipeId),
            entity.Day,
            entity.Label,
            entity.SortOrder,
            entity.UpdatedAtUtc);
    }
}

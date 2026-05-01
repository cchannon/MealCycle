using System.Text.Json;
using Azure.Data.Tables;
using MealCycle.Application.Configuration;
using MealCycle.Application.Interfaces;
using MealCycle.Domain.Recipes;
using MealCycle.Infrastructure.Data;
using MealCycle.Infrastructure.Data.Tables;
using Microsoft.Extensions.Options;

namespace MealCycle.Infrastructure.Repositories;

public sealed class AzureTableRecipeRepository : IRecipeRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly TableClient _tableClient;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public AzureTableRecipeRepository(TableServiceClient tableServiceClient, IOptions<AzureStorageOptions> options)
    {
        var tableName = options.Value.RecipesTableName;
        _tableClient = tableServiceClient.GetTableClient(tableName);
    }

    public async Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        var recipes = new List<Recipe>();
        await foreach (var entity in _tableClient.QueryAsync<RecipeTableEntity>(cancellationToken: cancellationToken))
        {
            recipes.Add(ToDomain(entity));
        }

        return recipes
            .OrderByDescending(recipe => recipe.UpdatedAtUtc)
            .ToList();
    }

    public async Task<Recipe?> GetAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        var response = await _tableClient.GetEntityIfExistsAsync<RecipeTableEntity>(
            TableEntityConstants.RecipePartitionKey,
            recipeId.ToString("N"),
            cancellationToken: cancellationToken);

        return response.HasValue ? ToDomain(response.Value!) : null;
    }

    public async Task<Recipe> UpsertAsync(Recipe recipe, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        await _tableClient.UpsertEntityAsync(ToEntity(recipe), cancellationToken: cancellationToken);
        return recipe;
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
            await foreach (var _ in _tableClient.QueryAsync<RecipeTableEntity>(maxPerPage: 1, cancellationToken: cancellationToken))
            {
                hasRows = true;
                break;
            }

            if (!hasRows)
            {
                foreach (var seedRecipe in SeedData.Recipes)
                {
                    await _tableClient.UpsertEntityAsync(ToEntity(seedRecipe), cancellationToken: cancellationToken);
                }
            }

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private static RecipeTableEntity ToEntity(Recipe recipe)
    {
        return new RecipeTableEntity
        {
            PartitionKey = TableEntityConstants.RecipePartitionKey,
            RowKey = recipe.Id.ToString("N"),
            Title = recipe.Title,
            IngredientsJson = JsonSerializer.Serialize(recipe.Ingredients, JsonOptions),
            StepsJson = JsonSerializer.Serialize(recipe.Steps, JsonOptions),
            UpdatedAtUtc = recipe.UpdatedAtUtc,
        };
    }

    private static Recipe ToDomain(RecipeTableEntity entity)
    {
        var ingredients = JsonSerializer.Deserialize<List<RecipeIngredient>>(entity.IngredientsJson, JsonOptions) ?? [];
        var steps = JsonSerializer.Deserialize<List<string>>(entity.StepsJson, JsonOptions) ?? [];

        return new Recipe(
            Guid.Parse(entity.RowKey),
            entity.Title,
            ingredients,
            steps,
            entity.UpdatedAtUtc);
    }
}

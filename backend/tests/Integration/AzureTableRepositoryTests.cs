using Azure;
using Azure.Data.Tables;
using FluentAssertions;
using MealCycle.Application.Configuration;
using MealCycle.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace MealCycle.IntegrationTests;

public sealed class AzureTableRepositoryTests : IClassFixture<AzuriteTableFixture>
{
    private readonly AzuriteTableFixture _fixture;

    public AzureTableRepositoryTests(AzuriteTableFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListAsync_WhenRepositoryIsInitialized_SeedsDefaultRecipes()
    {
        if (!_fixture.IsAzuriteAvailable)
        {
            return;
        }

        var repository = new AzureTableRecipeRepository(_fixture.TableServiceClient, _fixture.Options);

        var recipes = await repository.ListAsync(CancellationToken.None);

        recipes.Should().HaveCountGreaterThanOrEqualTo(4);
        recipes.Should().Contain(recipe => recipe.Title == "Weeknight Chili");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpsertAsync_WhenMealPlanItemIsSaved_PersistsAndRoundTrips()
    {
        if (!_fixture.IsAzuriteAvailable)
        {
            return;
        }

        var repository = new AzureTableMealPlanRepository(_fixture.TableServiceClient, _fixture.Options);
        var mealPlanId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        var saved = await repository.UpsertAsync(
            new MealCycle.Domain.MealPlans.MealPlanItem(mealPlanId, recipeId, "Thursday", "Integration Meal", 9),
            CancellationToken.None);

        var allItems = await repository.ListAsync(CancellationToken.None);
        var reloaded = allItems.Single(item => item.Id == mealPlanId);

        saved.Id.Should().Be(mealPlanId);
        reloaded.RecipeId.Should().Be(recipeId);
        reloaded.Day.Should().Be("Thursday");
        reloaded.Label.Should().Be("Integration Meal");
        reloaded.SortOrder.Should().Be(9);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SetStepCompletionAsync_WhenStepToggled_ReflectsLatestState()
    {
        if (!_fixture.IsAzuriteAvailable)
        {
            return;
        }

        var repository = new AzureTableCookProgressRepository(_fixture.TableServiceClient, _fixture.Options);
        var mealPlanItemId = Guid.NewGuid();

        await repository.SetStepCompletionAsync(mealPlanItemId, 2, true, CancellationToken.None);
        await repository.SetStepCompletionAsync(mealPlanItemId, 5, true, CancellationToken.None);

        var completedBeforeUnset = await repository.GetCompletedStepIndexesAsync(mealPlanItemId, CancellationToken.None);

        await repository.SetStepCompletionAsync(mealPlanItemId, 2, false, CancellationToken.None);
        var completedAfterUnset = await repository.GetCompletedStepIndexesAsync(mealPlanItemId, CancellationToken.None);

        completedBeforeUnset.Should().BeEquivalentTo([2, 5]);
        completedAfterUnset.Should().BeEquivalentTo([5]);
    }
}

public sealed class AzuriteTableFixture : IAsyncLifetime
{
    private readonly string _recipesTableName = $"itrecipes{Guid.NewGuid():N}";
    private readonly string _mealPlanTableName = $"itmealplan{Guid.NewGuid():N}";
    private readonly string _cookProgressTableName = $"itcookprogress{Guid.NewGuid():N}";

    public TableServiceClient TableServiceClient { get; } = new("UseDevelopmentStorage=true");

    public IOptions<AzureStorageOptions> Options { get; private set; } = null!;

    public bool IsAzuriteAvailable { get; private set; }

    public async Task InitializeAsync()
    {
        var config = new AzureStorageOptions
        {
            TableServiceUri = "http://127.0.0.1:10002/devstoreaccount1",
            RecipesTableName = _recipesTableName,
            MealPlanTableName = _mealPlanTableName,
            CookProgressTableName = _cookProgressTableName,
        };

        Options = Microsoft.Extensions.Options.Options.Create(config);

        try
        {
            await TableServiceClient.GetTableClient(_recipesTableName).CreateIfNotExistsAsync();
            await TableServiceClient.GetTableClient(_mealPlanTableName).CreateIfNotExistsAsync();
            await TableServiceClient.GetTableClient(_cookProgressTableName).CreateIfNotExistsAsync();
            IsAzuriteAvailable = true;
        }
        catch (Exception)
        {
            IsAzuriteAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (!IsAzuriteAvailable)
        {
            return;
        }

        await DeleteTableIfExistsAsync(_recipesTableName);
        await DeleteTableIfExistsAsync(_mealPlanTableName);
        await DeleteTableIfExistsAsync(_cookProgressTableName);
    }

    private async Task DeleteTableIfExistsAsync(string tableName)
    {
        try
        {
            await TableServiceClient.DeleteTableAsync(tableName);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
        }
    }
}

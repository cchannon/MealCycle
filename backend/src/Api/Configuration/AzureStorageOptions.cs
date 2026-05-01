namespace MealCycle.Api.Configuration;

public sealed class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";

    public string TableServiceUri { get; set; } = string.Empty;

    public string RecipesTableName { get; set; } = "recipes";

    public string MealPlanTableName { get; set; } = "meal-plan-items";

    public string CookProgressTableName { get; set; } = "cook-progress";
}

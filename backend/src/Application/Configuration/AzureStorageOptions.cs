namespace MealCycle.Application.Configuration;

public sealed class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";

    public string TableServiceUri { get; set; } = string.Empty;

    public string RecipesTableName { get; set; } = "recipes";

    public string MealPlanTableName { get; set; } = "mealplanitems";

    public string CookProgressTableName { get; set; } = "cookprogress";
}

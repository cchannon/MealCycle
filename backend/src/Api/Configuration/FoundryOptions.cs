namespace MealCycle.Api.Configuration;

public sealed class FoundryOptions
{
    public const string SectionName = "Foundry";

    public string Endpoint { get; set; } = string.Empty;

    public string ProjectName { get; set; } = string.Empty;

    public string ChatDeploymentName { get; set; } = string.Empty;

    public string EmbeddingDeploymentName { get; set; } = string.Empty;
}

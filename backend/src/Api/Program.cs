using MealCycle.Api.Configuration;
using MealCycle.Application.Configuration;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection(AzureStorageOptions.SectionName));
builder.Services.Configure<FoundryOptions>(builder.Configuration.GetSection(FoundryOptions.SectionName));

builder.Services.AddSingleton(sp =>
{
    var storageOptions = sp.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
    if (string.IsNullOrWhiteSpace(storageOptions.TableServiceUri))
    {
        throw new InvalidOperationException("AzureStorage:TableServiceUri must be configured.");
    }

    return new TableServiceClient(new Uri(storageOptions.TableServiceUri), new DefaultAzureCredential());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<MealCycle.Application.Interfaces.IRecipeRepository, MealCycle.Infrastructure.Repositories.AzureTableRecipeRepository>();
builder.Services.AddScoped<MealCycle.Application.Services.RecipeService>();
builder.Services.AddScoped<MealCycle.Application.Interfaces.IMealPlanRepository, MealCycle.Infrastructure.Repositories.AzureTableMealPlanRepository>();
builder.Services.AddScoped<MealCycle.Application.Services.MealPlanService>();
builder.Services.AddScoped<MealCycle.Application.Interfaces.ICookProgressRepository, MealCycle.Infrastructure.Repositories.AzureTableCookProgressRepository>();
builder.Services.AddScoped<MealCycle.Application.Services.CookModeService>();
builder.Services.AddScoped<MealCycle.Application.Services.ShoppingListService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("frontend");

app.MapControllers();
app.MapGet(
    "/health",
    (IOptions<AzureStorageOptions> storage, IOptions<FoundryOptions> foundry) =>
    {
        return Results.Ok(
            new
            {
                status = "ok",
                infrastructureReadiness = new
                {
                    persistenceProvider = "AzureTable",
                    tableStorageConfigured =
                        !string.IsNullOrWhiteSpace(storage.Value.TableServiceUri)
                        && !string.IsNullOrWhiteSpace(storage.Value.RecipesTableName)
                        && !string.IsNullOrWhiteSpace(storage.Value.MealPlanTableName)
                        && !string.IsNullOrWhiteSpace(storage.Value.CookProgressTableName),
                    foundryConfigured =
                        !string.IsNullOrWhiteSpace(foundry.Value.Endpoint)
                        && !string.IsNullOrWhiteSpace(foundry.Value.ProjectName)
                        && !string.IsNullOrWhiteSpace(foundry.Value.ChatDeploymentName),
                },
            });
    });

app.Run();

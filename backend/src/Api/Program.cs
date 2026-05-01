using MealCycle.Api.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<PersistenceOptions>(builder.Configuration.GetSection(PersistenceOptions.SectionName));
builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection(AzureStorageOptions.SectionName));
builder.Services.Configure<FoundryOptions>(builder.Configuration.GetSection(FoundryOptions.SectionName));

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<MealCycle.Application.Interfaces.IRecipeRepository, MealCycle.Infrastructure.Repositories.InMemoryRecipeRepository>();
builder.Services.AddScoped<MealCycle.Application.Services.RecipeService>();
builder.Services.AddSingleton<MealCycle.Application.Interfaces.IMealPlanRepository, MealCycle.Infrastructure.Repositories.InMemoryMealPlanRepository>();
builder.Services.AddScoped<MealCycle.Application.Services.MealPlanService>();
builder.Services.AddSingleton<MealCycle.Application.Interfaces.ICookProgressRepository, MealCycle.Infrastructure.Repositories.InMemoryCookProgressRepository>();
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
    (IOptions<PersistenceOptions> persistence, IOptions<AzureStorageOptions> storage, IOptions<FoundryOptions> foundry) =>
    {
        var usingInMemoryPersistence = string.Equals(
            persistence.Value.Provider,
            "InMemory",
            StringComparison.OrdinalIgnoreCase);

        return Results.Ok(
            new
            {
                status = "ok",
                infrastructureReadiness = new
                {
                    persistenceProvider = persistence.Value.Provider,
                    usingInMemoryPersistence,
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

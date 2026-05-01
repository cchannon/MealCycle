using MealCycle.Domain.Recipes;

namespace MealCycle.Application.Interfaces;

public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken cancellationToken);

    Task<Recipe?> GetAsync(Guid recipeId, CancellationToken cancellationToken);

    Task<Recipe> UpsertAsync(Recipe recipe, CancellationToken cancellationToken);
}

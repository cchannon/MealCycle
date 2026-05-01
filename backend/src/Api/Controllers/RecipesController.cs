using MealCycle.Application.Features.Recipes;
using MealCycle.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealCycle.Api.Controllers;

[ApiController]
[Route("api/recipes")]
public sealed class RecipesController : ControllerBase
{
    private readonly RecipeService _recipeService;

    public RecipesController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecipeModel>>> ListAsync(CancellationToken cancellationToken)
    {
        var recipes = await _recipeService.ListAsync(cancellationToken);
        return Ok(recipes);
    }

    [HttpGet("{recipeId:guid}")]
    public async Task<ActionResult<RecipeModel>> GetAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        var recipe = await _recipeService.GetAsync(recipeId, cancellationToken);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeModel>> CreateAsync([FromBody] CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return ValidationProblem("Title is required.");
        }

        var created = await _recipeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAsync), new { recipeId = created.Id }, created);
    }
}

using MealCycle.Application.Features.MealPlans;
using MealCycle.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealCycle.Api.Controllers;

[ApiController]
[Route("api/meal-plan")]
public sealed class MealPlanController : ControllerBase
{
    private readonly MealPlanService _mealPlanService;

    public MealPlanController(MealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MealPlanItemModel>>> ListAsync(CancellationToken cancellationToken)
    {
        var mealPlanItems = await _mealPlanService.ListAsync(cancellationToken);
        return Ok(mealPlanItems);
    }

    [HttpPost]
    public async Task<ActionResult<MealPlanItemModel>> CreateAsync([FromBody] CreateMealPlanItemRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Day))
        {
            return ValidationProblem("Day is required.");
        }

        if (request.RecipeId == Guid.Empty)
        {
            return ValidationProblem("RecipeId is required.");
        }

        var created = await _mealPlanService.CreateAsync(request, cancellationToken);
        if (created is null)
        {
            return NotFound("Recipe was not found.");
        }

        return CreatedAtAction(nameof(ListAsync), new { mealPlanItemId = created.Id }, created);
    }

    [HttpPut("reorder")]
    public async Task<ActionResult<IReadOnlyList<MealPlanItemModel>>> ReorderAsync([FromBody] ReorderMealPlanRequest request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
        {
            return ValidationProblem("At least one item is required to reorder the plan.");
        }

        var reordered = await _mealPlanService.ReorderAsync(request, cancellationToken);
        return Ok(reordered);
    }
}
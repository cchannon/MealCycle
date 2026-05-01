using MealCycle.Application.Features.CookMode;
using MealCycle.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealCycle.Api.Controllers;

[ApiController]
[Route("api/cook-mode")]
public sealed class CookModeController : ControllerBase
{
    private readonly CookModeService _cookModeService;

    public CookModeController(CookModeService cookModeService)
    {
        _cookModeService = cookModeService;
    }

    [HttpGet("{mealPlanItemId:guid}")]
    public async Task<ActionResult<CookModeSessionModel>> GetSessionAsync(Guid mealPlanItemId, CancellationToken cancellationToken)
    {
        var session = await _cookModeService.GetSessionAsync(mealPlanItemId, cancellationToken);
        return session is null ? NotFound() : Ok(session);
    }

    [HttpPut("{mealPlanItemId:guid}/steps/{stepIndex:int}")]
    public async Task<ActionResult<CookModeSessionModel>> SetStepCompletionAsync(
        Guid mealPlanItemId,
        int stepIndex,
        [FromBody] UpdateCookStepRequest request,
        CancellationToken cancellationToken)
    {
        if (stepIndex < 0)
        {
            return ValidationProblem("Step index must be zero or greater.");
        }

        try
        {
            var updatedSession = await _cookModeService.SetStepCompletionAsync(mealPlanItemId, stepIndex, request.IsCompleted, cancellationToken);
            return updatedSession is null ? NotFound() : Ok(updatedSession);
        }
        catch (ArgumentOutOfRangeException)
        {
            return ValidationProblem("Step index is outside the recipe step range.");
        }
    }
}
using MealCycle.Application.Features.ShoppingLists;
using MealCycle.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealCycle.Api.Controllers;

[ApiController]
[Route("api/shopping-list")]
public sealed class ShoppingListController : ControllerBase
{
    private readonly ShoppingListService _shoppingListService;

    public ShoppingListController(ShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpPost("preview")]
    public async Task<ActionResult<IReadOnlyList<ShoppingListItemModel>>> PreviewAsync([FromBody] ShoppingListPreviewRequest request, CancellationToken cancellationToken)
    {
        if (request.MealPlanItemIds.Count == 0)
        {
            return ValidationProblem("Select at least one meal before generating the list.");
        }

        var preview = await _shoppingListService.PreviewAsync(request, cancellationToken);
        return Ok(preview);
    }
}
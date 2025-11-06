using Inventory.Application.Features.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ISender _mediator;
    public InventoryController(ISender mediator) => _mediator = mediator;

    [HttpPost("restock")]
    public async Task<IActionResult> Restock(RestockCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve(ReserveStockCommand cmd)
    {
        var success = await _mediator.Send(cmd);
        return success ? Ok() : BadRequest("Not enough stock");
    }
}


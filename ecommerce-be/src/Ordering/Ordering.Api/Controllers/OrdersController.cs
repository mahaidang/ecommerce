using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common;
using Ordering.Application.Orders;
using Ordering.Application.Orders.Command;

namespace Ordering.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator, IOrderingDbContext db) { _mediator = mediator; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand cmd, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(cmd, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Stock check failed", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    // GET /api/Orders?userId=...&page=&pageSize=
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string status = "", CancellationToken ct = default)
    {
        if (userId == Guid.Empty) return BadRequest("userId is required.");
        var result = await _mediator.Send(new GetOrdersByUserQuery(userId, page, pageSize, status), ct);
        return Ok(result);
    }

    // GET /api/Orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetOrderDetailQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }


    // POST /api/Orders/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] string? reason, CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new CancelOrderCommand(id, reason), ct);
        return ok ? NoContent() : BadRequest("Cannot cancel this order.");
    }

    // POST /api/Orders/{id}/status?to=Paid
    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromQuery] string to, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(to)) return BadRequest("Missing 'to' status.");
        var ok = await _mediator.Send(new UpdateStatusCommand(id, to), ct);
        return ok ? NoContent() : BadRequest("Invalid status transition.");
    }

}

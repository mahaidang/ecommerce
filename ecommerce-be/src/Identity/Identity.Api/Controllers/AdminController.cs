using Identity.Application.Features.Commands.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{orderId}/approve")]
        public async Task<IActionResult> Approve(Guid orderId, CancellationToken ct, string? note)
        {
            await _mediator.Send(new ApproveOrderCommand(orderId, true, note), ct);
            return Ok(new { message = "Order approved and event published" });
        }

        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> Reject(Guid orderId, CancellationToken ct, string? note)
        {
            await _mediator.Send(new ApproveOrderCommand(orderId, false, note));
            return Ok(new { message = "Order rejected and event published" });
        }
    }
}

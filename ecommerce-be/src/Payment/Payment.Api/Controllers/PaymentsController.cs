using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Commands;

namespace Payment.Api.Controllers;

[ApiController]
[Authorize]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("sepay/{orderId:guid}")]
    public async Task<IActionResult> CreatePayment(Guid orderId, string orderNo, [FromBody] decimal amount)
    {
        var result = await _mediator.Send(new CreateSePayPaymentCommand(orderId, orderNo, amount));
        return Ok(result);
    }
}

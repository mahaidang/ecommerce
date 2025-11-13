using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Commands;

namespace Payment.Api.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("sepay/{orderId:guid}")]
    public async Task<IActionResult> CreatePayment(Guid orderId, [FromBody] decimal amount)
    {
        var result = await _mediator.Send(new CreateSePayPaymentCommand(orderId,"", amount));
        return Ok(result);
    }
}

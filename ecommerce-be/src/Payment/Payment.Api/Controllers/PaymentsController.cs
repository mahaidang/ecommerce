using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Commands;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("sepay/{orderId:guid}")]
    public async Task<IActionResult> Create(Guid orderId)
    {
        var result = await _mediator.Send(new CreateSePayPaymentCommand(orderId));
        return Ok(result);
    }
}

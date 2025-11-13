using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Commands;
using Payment.Application.Features.Dtos;

namespace Payment.Api.Controllers;

[ApiController]
[Route("webhook/sepay")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhookController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] SePayWebhookDto payload)
    {
        try
        {
            await _mediator.Send(new HandleSePayWebhookCommand(payload));
            // Phản hồi theo spec SePay: trả success true và status 201 (hoặc 200)
            return StatusCode(201, new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook error: {ex.Message}");
            return BadRequest();
        }
    }
}

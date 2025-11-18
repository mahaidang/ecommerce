using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Commands;
using Payment.Application.Features.Dtos;

namespace Payment.Api.Controllers;

[ApiController]
[Authorize]
[Route("webhook/sepay")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _config;


    public WebhookController(IMediator mediator, IConfiguration config)
    {
        _config = config;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] SePayWebhookDto payload)
    {
        var sepaySection = _config.GetSection("SePay");
        var secret = sepaySection["Secret"];

        // Lấy API Key từ header
        if (!Request.Headers.TryGetValue("Authorization", out var apiKey))
        {
            return Unauthorized("Missing API Key");
        }

        // So sánh với key cấu hình
        if (apiKey != secret)
        {
            return Unauthorized("Invalid API Key");
        }
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

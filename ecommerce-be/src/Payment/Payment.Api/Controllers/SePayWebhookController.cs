using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/webhooks/sepay")]
public class SePayWebhookController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IMediator _mediator;

    public SePayWebhookController(IMediator mediator, IConfiguration config)
    {
        _mediator = mediator;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] SePayWebhookDto payload,
                                             [FromHeader(Name = "x-sepay-signature")] string signature)
    {
        var secret = _config["SePay:Secret"];
        var raw = JsonSerializer.Serialize(payload);
        var computed = HmacHelper.ComputeHmac(secret, raw);

        if (!computed.Equals(signature, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Invalid signature");

        await _mediator.Send(new HandleSePayWebhookCommand(payload));
        return Ok("OK");
    }
}

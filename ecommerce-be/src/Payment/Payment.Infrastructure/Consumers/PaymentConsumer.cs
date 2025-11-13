using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payment.Application.Features.Commands;
using Shared.Contracts.Events;

namespace Payment.Infrastructure.Consumers;

public class PaymentConsumer : IConsumer<EventEnvelope<CmdPaymentRequest>>
{
    private readonly ISender _mediator;
    private readonly ILogger<CmdPaymentRequest> _log;


    public PaymentConsumer(ISender mediator, ILogger<CmdPaymentRequest> log)
    {
        _mediator = mediator;
        _log = log;
    }
    public async Task Consume(ConsumeContext<EventEnvelope<CmdPaymentRequest>> context)
    {
        var req = context.Message;
        _log.LogError("saga -> payment");
        await _mediator.Send(new CreateSePayPaymentCommand(req.OrderId, req.OrderNo, req.Data.Amount));
    }
}

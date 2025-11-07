using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Orders.Command;
using Shared.Contracts.Events;

namespace Ordering.Infrastructure.Consumers;

public class UpdateStatusConsumer : IConsumer<EventEnvelope<CmdOrderUpdateStatus>>
{
    private readonly ISender _mediator;
    private readonly ILogger<CmdOrderUpdateStatus> _log;

    public UpdateStatusConsumer(ISender mediator, ILogger<CmdOrderUpdateStatus> log)
    {
        _mediator = mediator;
        _log = log;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<CmdOrderUpdateStatus>> context)
    {
        var req = context.Message;
        _log.LogInformation("received update status", req.OrderId);
        await _mediator.Send(new UpdateStatusCommand(req.OrderId, req.Data.NewStatus));
    }
}

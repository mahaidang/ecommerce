using Inventory.Application.Features.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace Inventory.Infrastructure.Consumers;

public class CommitStockConsumer : IConsumer<EventEnvelope<CmdInventoryCommit>>
{
    private readonly ISender _mediator;
    private readonly ILogger<CmdInventoryCommit> _log;


    public CommitStockConsumer(ISender mediator, ILogger<CmdInventoryCommit> log)
    {
        _mediator = mediator;
        _log = log;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<CmdInventoryCommit>> context)
    {
        var req = context.Message;
        await _mediator.Send(new CommitStockCommand(req.OrderId));
    }
}
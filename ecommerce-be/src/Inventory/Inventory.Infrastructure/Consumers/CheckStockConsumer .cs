using Inventory.Application.Features.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace Inventory.Infrastructure.Consumers;

public class CheckStockConsumer : IConsumer<EventEnvelope<CmdInventoryReserve>>
{
    private readonly ISender _mediator;
    private readonly ILogger<CmdInventoryReserve> _log;


    public CheckStockConsumer(ISender mediator, ILogger<CmdInventoryReserve> log)
    {
        _mediator = mediator;
        _log = log;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<CmdInventoryReserve>> context)
    {
        var req = context.Message;
        _log.LogInformation("CmdInventoryReserve", req.OrderId);
        await _mediator.Send(new ReserveStockCommand(req.OrderId,
        req.Data.Items.Select(i => new ReservedItem(i.ProductId, i.Quantity)).ToList(),
        req.CorrelationId,
        req.EventType,
        req.UtcNow));
    }
}
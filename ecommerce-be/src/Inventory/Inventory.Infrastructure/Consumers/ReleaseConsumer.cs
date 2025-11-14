using Inventory.Application.Features.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace Inventory.Infrastructure.Consumers;

public class ReleaseConsumer : IConsumer<EventEnvelope<CmdInventoryRelease>>
{
    private readonly ILogger<ReleaseConsumer> _log;
    private readonly ISender _mediator;

    public ReleaseConsumer(ISender mediator, ILogger<ReleaseConsumer> log)
    {
        _mediator = mediator;
        _log = log;
    }


    public async Task Consume(ConsumeContext<EventEnvelope<CmdInventoryRelease>> context)
    {
        var env = context.Message;
        await _mediator.Send(new ReleaseStockCommand(env.Data.Items.ToList()));
    }

}
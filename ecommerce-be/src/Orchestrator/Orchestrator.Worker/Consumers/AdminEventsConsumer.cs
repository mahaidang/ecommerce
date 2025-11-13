using MassTransit;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Orchestrator.Worker.Consumers;

public class AdminEventsConsumer : IConsumer<EventEnvelope<OrderApprovalResult>>
{

    private readonly ILogger<AdminEventsConsumer> _log;
    public AdminEventsConsumer(ILogger<AdminEventsConsumer> log) => _log = log;
    public async Task Consume(ConsumeContext<EventEnvelope<OrderApprovalResult>> context)
    {
        var env = context.Message;
        if (env.Data.Approved)
        {
            var update = new EventEnvelope<CmdOrderUpdateStatus>(
                Rk.CmdOrderUpdateStatus,
                env.CorrelationId,
                env.OrderId,
                env.OrderNo,
                new CmdOrderUpdateStatus(env.OrderId, "Shipping"),
                DateTime.UtcNow
            );
            await context.Publish(update);
            _log.LogError("Saga → Order: approved");
            var commit = new EventEnvelope<CmdInventoryCommit>(
                Rk.CmdOrderUpdateStatus,
                env.CorrelationId,
                env.OrderId,
                env.OrderNo,
                new CmdInventoryCommit(),
                DateTime.UtcNow
            );
            await context.Publish(commit);
            _log.LogError("Saga → inventory: commit");

        }
        else
        {
            _log.LogError("Saga → Order: rejected", env.Data.Note);
        }
        
    }
}

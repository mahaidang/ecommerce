using MassTransit;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Orchestrator.Worker.Consumers;

internal class AdminEventsConsumer : IConsumer<EventEnvelope<OrderApprovalResult>>
{
    public async Task Consume(ConsumeContext<EventEnvelope<OrderApprovalResult>> context)
    {
        var env = context.Message;
        object? cmd = null;
        if (env.Data.Approved)
        {
            cmd = new CmdOrderUpdateStatus(env.OrderId, "Shipping");
        }
        else
        {
            Console.WriteLine($"❌ Order {env.OrderId} rejected by admin. Note: {env.Data.Note}");
        }
        var update = new EventEnvelope<object?>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            cmd,
            DateTime.UtcNow
        );
        await context.Publish(update);
    }
}

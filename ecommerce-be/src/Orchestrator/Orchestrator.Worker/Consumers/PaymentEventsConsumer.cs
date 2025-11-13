using MassTransit;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Orchestrator.Worker.Consumers;

public class PaymentEventsConsumer :
    IConsumer<EventEnvelope<PaymentSucceededData>>,
    IConsumer<EventEnvelope<PaymentFailedData>>
{
    private readonly ILogger<PaymentEventsConsumer> _log;
    public PaymentEventsConsumer(ILogger<PaymentEventsConsumer> log) => _log = log;

    public async Task Consume(ConsumeContext<EventEnvelope<PaymentSucceededData>> context)
    {
        var env = context.Message;
        _log.LogError("Payment -> saga");

        var update = new EventEnvelope<CmdOrderUpdateStatus>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            env.OrderNo,
            new CmdOrderUpdateStatus(env.OrderId, "Shipped"),
            DateTime.UtcNow
        );
        await context.Publish(update);
        _log.LogError("Saga -> order: update");
        var commit = new EventEnvelope<CmdInventoryCommit>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            env.OrderNo,
            new CmdInventoryCommit(),
            DateTime.UtcNow
        );
        await context.Publish(commit);
        _log.LogError("Saga -> inventory: commit");
    }

    public async Task Consume(ConsumeContext<EventEnvelope<PaymentFailedData>> context)
    {
        //var env = context.Message;
        //_log.LogWarning("💳 PaymentFailed: {OrderId} - {Reason}", env.OrderId, env.Data.Reason);

        //var rel = new EventEnvelope<CmdInventoryRelease>(
        //    Rk.CmdInventoryRelease, env.CorrelationId, env.OrderId,
        //    new CmdInventoryRelease(env.OrderId, new List<ReservedItem>()),
        //    DateTime.UtcNow
        //);
        //await context.Publish(rel);

        //var cancel = new EventEnvelope<CmdOrderUpdateStatus>(
        //    Rk.CmdOrderUpdateStatus, env.CorrelationId, env.OrderId,
        //    new CmdOrderUpdateStatus(env.OrderId, "Cancelled"),
        //    DateTime.UtcNow
        //);
        //await context.Publish(cancel);
    }
}

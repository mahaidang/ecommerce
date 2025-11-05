using MassTransit;
using OrchestratorService.Worker.Messaging;

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
        _log.LogInformation("💰 PaymentSucceeded: {OrderId}", env.OrderId);

        var confirm = new EventEnvelope<object>(Rk.OrderConfirmed, env.CorrelationId, env.OrderId, new { }, DateTime.UtcNow);
        await context.Publish(confirm);

        var update = new EventEnvelope<CmdOrderUpdateStatus>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            new CmdOrderUpdateStatus(env.OrderId, "Paid"),
            DateTime.UtcNow
        );
        await context.Publish(update);
    }

    public async Task Consume(ConsumeContext<EventEnvelope<PaymentFailedData>> context)
    {
        var env = context.Message;
        _log.LogWarning("💳 PaymentFailed: {OrderId} - {Reason}", env.OrderId, env.Data.Reason);

        var rel = new EventEnvelope<CmdInventoryRelease>(
            Rk.CmdInventoryRelease, env.CorrelationId, env.OrderId,
            new CmdInventoryRelease(env.OrderId, new List<ReservedItem>()),
            DateTime.UtcNow
        );
        await context.Publish(rel);

        var cancel = new EventEnvelope<CmdOrderUpdateStatus>(
            Rk.CmdOrderUpdateStatus, env.CorrelationId, env.OrderId,
            new CmdOrderUpdateStatus(env.OrderId, "Cancelled"),
            DateTime.UtcNow
        );
        await context.Publish(cancel);
    }
}

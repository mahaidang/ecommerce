using MassTransit;
using OrchestratorService.Worker.Messaging;

namespace Orchestrator.Worker.Consumers;

public class InventoryEventsConsumer : IConsumer<EventEnvelope<InventoryReservedData>>, IConsumer<EventEnvelope<InventoryFailedData>>
{
    private readonly ILogger<InventoryEventsConsumer> _log;
    public InventoryEventsConsumer(ILogger<InventoryEventsConsumer> log) => _log = log;

    public async Task Consume(ConsumeContext<EventEnvelope<InventoryReservedData>> context)
    {
        var env = context.Message;
        _log.LogInformation("✅ InventoryReserved: {OrderId}", env.OrderId);

        var cmd = new EventEnvelope<CmdPaymentRequest>(
            Rk.CmdPaymentRequest,
            env.CorrelationId,
            env.OrderId,
            new CmdPaymentRequest(env.OrderId, 0, "VND"),
            DateTime.UtcNow
        );
        await context.Publish(cmd);
    }

    public async Task Consume(ConsumeContext<EventEnvelope<InventoryFailedData>> context)
    {
        var env = context.Message;
        _log.LogWarning("❌ InventoryFailed: {OrderId}, Reason={Reason}", env.OrderId, env.Data.Reason);

        var cancel = new EventEnvelope<CmdOrderUpdateStatus>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            new CmdOrderUpdateStatus(env.OrderId, "Cancelled"),
            DateTime.UtcNow
        );
        await context.Publish(cancel);
    }
}
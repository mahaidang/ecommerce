using MassTransit;
using Orchestrator.Worker.Repositories;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;


namespace Orchestrator.Worker.Consumers;

public class InventoryEventsConsumer : IConsumer<EventEnvelope<InventoryReservedData>>, IConsumer<EventEnvelope<InventoryFailedData>>
{
    private readonly ILogger<OrderCreatedConsumer> _log;
    private readonly OrderSagaRepository _repo;
    public InventoryEventsConsumer(ILogger<OrderCreatedConsumer> log, OrderSagaRepository repo)
    {
        _log = log;
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<InventoryReservedData>> context)
    {
        var env = context.Message;
        _log.LogError("Inven to saga");

        var saga = await _repo.GetByOrderIdAsync(env.OrderId);
        if (saga == null) return;

        saga.InventoryReserved = true;
        saga.Status = "AwaitingPayment";
        await _repo.UpdateAsync(saga);

        if (env.Pay)
        {
            var cmd = new EventEnvelope<CmdPaymentRequest>(
                Rk.CmdPaymentRequest,
                env.CorrelationId,
                env.OrderId,
                env.OrderNo,
                new CmdPaymentRequest(saga.Amount, "VND"),
                DateTime.UtcNow,
                env.Pay
            );
            await context.Publish(cmd);
            _log.LogError("saga -> payment");
        }
    }

    public async Task Consume(ConsumeContext<EventEnvelope<InventoryFailedData>> context)
    {
        var env = context.Message;
        _log.LogWarning("❌ InventoryFailed: {OrderId}, Reason={Reason}", env.OrderId, env.Data.Reason);

        var cancel = new EventEnvelope<CmdOrderUpdateStatus>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            env.OrderNo,
            new CmdOrderUpdateStatus("Cancelled"),
            DateTime.UtcNow
        );
        await context.Publish(cancel);
    }
}
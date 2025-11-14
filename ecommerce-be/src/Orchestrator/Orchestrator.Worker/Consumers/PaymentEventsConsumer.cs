using MassTransit;
using Orchestrator.Worker.Repositories;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Orchestrator.Worker.Consumers;

public class PaymentEventsConsumer :
    IConsumer<EventEnvelope<PaymentSucceededData>>,
    IConsumer<EventEnvelope<PaymentFailedData>>
{
    private readonly ILogger<PaymentEventsConsumer> _log;
    private readonly OrderSagaRepository _repo;

    public PaymentEventsConsumer(ILogger<PaymentEventsConsumer> log, OrderSagaRepository repo)
    {
        _log = log;
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<PaymentSucceededData>> context)
    {
        var env = context.Message;
        _log.LogError("Payment -> saga");
        var saga = await _repo.GetByOrderIdAsync(env.OrderId);
        var items = new List<ItemData>();

        foreach (var i in saga.OrderSagaItems)
        {
            items.Add(new ItemData(
                i.ProductId,
                i.Quantity
            ));
        }

        var update = new EventEnvelope<CmdOrderUpdateStatus>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            env.OrderNo,
            new CmdOrderUpdateStatus("Shipped"),
            DateTime.UtcNow
        );
        await context.Publish(update);
        _log.LogError("Saga -> order: update");
        var commit = new EventEnvelope<CmdInventoryCommit>(
            Rk.CmdOrderUpdateStatus,
            env.CorrelationId,
            env.OrderId,
            env.OrderNo,
            new CmdInventoryCommit(items),
            DateTime.UtcNow
        );
        await context.Publish(commit);
        _log.LogError("Saga -> inventory: commit");
    }

    public async Task Consume(ConsumeContext<EventEnvelope<PaymentFailedData>> context)
    {
        var env = context.Message;
        _log.LogError("Payment -> saga");
        var saga = await _repo.GetByOrderIdAsync(env.OrderId );
        var items = new List<ItemData>();

        foreach (var i in saga.OrderSagaItems)
        {
            items.Add(new ItemData(
                i.ProductId,
                i.Quantity
            ));
        }

        var rel = new EventEnvelope<CmdInventoryRelease>(
            Rk.CmdInventoryRelease,
            env.CorrelationId,
            env.OrderId,
            env.OrderNo,
            new CmdInventoryRelease(items),
            DateTime.UtcNow
        );
        await context.Publish(rel);

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

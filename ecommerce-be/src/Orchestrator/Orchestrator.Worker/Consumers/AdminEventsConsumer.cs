using MassTransit;
using Orchestrator.Worker.Repositories;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Orchestrator.Worker.Consumers;

public class AdminEventsConsumer : IConsumer<EventEnvelope<OrderApprovalResult>>
{

    private readonly ILogger<AdminEventsConsumer> _log;
    private readonly OrderSagaRepository _repo;
    public AdminEventsConsumer(ILogger<AdminEventsConsumer> log, OrderSagaRepository repo)
    {
        _log = log;
        _repo = repo;
    }
    public async Task Consume(ConsumeContext<EventEnvelope<OrderApprovalResult>> context)
    {
        var env = context.Message;
        var saga = await _repo.GetByOrderIdAsync(env.OrderId);
        var items = new List<ItemData>();

        foreach (var i in saga.OrderSagaItems)
        {
            items.Add(new ItemData(
                i.ProductId,
                i.Quantity
            ));
        }

        if (env.Data.Approved)
        {
            var update = new EventEnvelope<CmdOrderUpdateStatus>(
                Rk.CmdOrderUpdateStatus,
                env.CorrelationId,
                env.OrderId,
                env.OrderNo,
                new CmdOrderUpdateStatus("Shipping"),
                DateTime.UtcNow
            );
            await context.Publish(update);
            _log.LogError("Saga → Order: approved");
            var commit = new EventEnvelope<CmdInventoryCommit>(
                Rk.CmdInventoryCommit,
                env.CorrelationId,
                env.OrderId,
                env.OrderNo,
                new CmdInventoryCommit(items),
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

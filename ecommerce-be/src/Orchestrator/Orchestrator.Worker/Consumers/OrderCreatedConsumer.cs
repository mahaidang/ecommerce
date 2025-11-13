using MassTransit;
using Orchestrator.Worker.Models;
using Orchestrator.Worker.Repositories;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;
namespace Orchestrator.Worker.Consumers;

public class OrderCreatedConsumer : IConsumer<EventEnvelope<OrderCreatedData>>
{
    private readonly ILogger<OrderCreatedConsumer> _log;
    private readonly OrderSagaRepository _repo;
    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> log, OrderSagaRepository repo)
    {
        _log = log;
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<OrderCreatedData>> context)
    {
        var env = context.Message;
        _log.LogError("Order -> Saga");
        var corrId = env.CorrelationId != Guid.Empty ? env.CorrelationId : Guid.NewGuid();

        // Tạo saga mới
        var saga = new OrderSagaState
        {
            Id = Guid.NewGuid(),
            OrderId = env.OrderId,
            Amount = env.Data.GrandTotal,
            CustomerId = env.Data.UserId,
            Status = "AwaitingInventory"
        };
        await _repo.SaveAsync(saga);

        if (env.Data?.Items == null)
        {
            _log.LogError("❌ env.Data.Items is null for OrderId={OrderId}", env.OrderId);
            return;
        }


        // Gửi command: cmd.inventory.reserve
        var reserve = new EventEnvelope<CmdInventoryReserve>(
            Rk.CmdInventoryReserve,
            corrId,
            env.OrderId,
            env.OrderNo,
            new CmdInventoryReserve(
                env.OrderId,
                env.Data.Items.Select(i => new ReservedItem(i.ProductId, i.Quantity)).ToList()
            ),
            DateTime.UtcNow,
            env.Pay
        );

        try
        {
            await context.Publish(reserve);
            _log.LogError("Saga -> Inventory");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "❌ Failed to publish CmdInventoryReserve");
        }

    }
}
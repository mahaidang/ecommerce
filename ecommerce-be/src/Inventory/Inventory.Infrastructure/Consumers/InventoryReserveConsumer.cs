using Inventory.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using static Inventory.Infrastructure.Consumers.InventoryReserveConsumer;

namespace Inventory.Infrastructure.Consumers;

public class InventoryReserveConsumer : IConsumer<EventEnvelope<CmdInventoryReserve>>
{
    private readonly ILogger<InventoryReserveConsumer> _log;
    private readonly IInventoryDbContext _db;

    public InventoryReserveConsumer(ILogger<InventoryReserveConsumer> log, IInventoryDbContext db)
    {
        _log = log;
        _db = db;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<CmdInventoryReserve>> context)
    {
        var env = context.Message;
        _log.LogInformation("📦 Received cmd.inventory.reserve for OrderId={OrderId}", env.OrderId);

        // TODO: kiểm tra tồn kho thật
        bool ok = true;

        if (ok)
        {
            var evt = new EventEnvelope<InventoryReservedData>(
                "inventory.stock.reserved",
                env.CorrelationId,
                env.OrderId,
                new InventoryReservedData(Guid.Empty, env.Data.Items.ToList()),
                DateTime.UtcNow
            );
            await context.Publish(evt);
            _log.LogInformation("✅ Stock reserved for Order {OrderId}", env.OrderId);
        }
        else
        {
            var evt = new EventEnvelope<InventoryFailedData>(
                "inventory.stock.failed",
                env.CorrelationId,
                env.OrderId,
                new InventoryFailedData("Out of stock"),
                DateTime.UtcNow
            );
            await context.Publish(evt);
            _log.LogWarning("❌ Stock reserve failed for Order {OrderId}", env.OrderId);
        }
    }

    // Contracts giống Orchestrator
    public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime OccurredAtUtc);
    public record ReservedItem(Guid ProductId, int Quantity);
    public record InventoryReservedData(Guid WarehouseId, IReadOnlyList<ReservedItem> Items);
    public record InventoryFailedData(string Reason);
    public record CmdInventoryReserve(Guid OrderId, IReadOnlyList<ReservedItem> Items);
}
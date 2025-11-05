using MassTransit;
using Microsoft.Extensions.Logging;
using static Inventory.Infrastructure.Consumers.InventoryReleaseConsumer;

namespace Inventory.Infrastructure.Consumers;

public class InventoryReleaseConsumer : IConsumer<EventEnvelope<CmdInventoryRelease>>
{
    private readonly ILogger<InventoryReleaseConsumer> _log;
    public InventoryReleaseConsumer(ILogger<InventoryReleaseConsumer> log) => _log = log;

    public async Task Consume(ConsumeContext<EventEnvelope<CmdInventoryRelease>> context)
    {
        var env = context.Message;
        _log.LogInformation("♻️ Received cmd.inventory.release for OrderId={OrderId}", env.OrderId);
        await Task.CompletedTask;
    }

    // Contracts
    public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime OccurredAtUtc);
    public record ReservedItem(Guid ProductId, int Quantity);
    public record CmdInventoryRelease(Guid OrderId, IReadOnlyList<ReservedItem> Items);
}
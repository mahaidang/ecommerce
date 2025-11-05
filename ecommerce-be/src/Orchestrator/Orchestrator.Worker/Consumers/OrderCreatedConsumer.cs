using MassTransit;
using OrchestratorService.Worker.Messaging;
namespace Orchestrator.Worker.Consumers;

public class OrderCreatedConsumer : IConsumer<EventEnvelope<OrderCreatedData>>
{
    private readonly ILogger<OrderCreatedConsumer> _log;
    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> log) => _log = log;

    public async Task Consume(ConsumeContext<EventEnvelope<OrderCreatedData>> context)
    {
        var env = context.Message;
        _log.LogInformation("📦 [Saga] Received OrderCreated: {OrderId}", env.OrderId);

        var corrId = env.CorrelationId != Guid.Empty ? env.CorrelationId : Guid.NewGuid();

        // Gửi command: cmd.inventory.reserve
        var reserve = new EventEnvelope<CmdInventoryReserve>(
            Rk.CmdInventoryReserve,
            corrId,
            env.OrderId,
            new CmdInventoryReserve(
                env.OrderId,
                env.Data.Items.Select(i => new ReservedItem(i.ProductId, i.Quantity)).ToList()
            ),
            DateTime.UtcNow
        );

        await context.Publish(reserve);

        // Gửi luôn yêu cầu thanh toán (tùy mô hình)
        var pay = new EventEnvelope<CmdPaymentRequest>(
            Rk.CmdPaymentRequest,
            corrId,
            env.OrderId,
            new CmdPaymentRequest(env.OrderId, env.Data.GrandTotal, env.Data.Currency ?? "VND"),
            DateTime.UtcNow
        );

        await context.Publish(pay);
    }
}
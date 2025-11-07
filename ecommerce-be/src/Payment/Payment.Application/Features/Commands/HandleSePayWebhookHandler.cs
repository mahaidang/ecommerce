using MediatR;

namespace Payment.Application.Features.Commands;

public class SePayWebhookDto
{
    public string OrderCode { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
    public string TransactionId { get; set; } = default!;
    public string Bank { get; set; } = default!;
}

public record HandleSePayWebhookCommand(SePayWebhookDto Payload) : IRequest;

public class HandleSePayWebhookHandler : IRequestHandler<HandleSePayWebhookCommand>
{
    private readonly IOrderRepository _repo;
    public HandleSePayWebhookHandler(IOrderRepository repo) => _repo = repo;

    public async Task Handle(HandleSePayWebhookCommand cmd, CancellationToken ct)
    {
        var data = cmd.Payload;
        var order = await _repo.GetByCodeAsync(data.OrderCode, ct);
        if (order == null) return;

        if (data.Status == "success" && data.Amount == order.TotalPrice)
        {
            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.PaymentMethod = "SePay";
            await _repo.UpdateAsync(order, ct);
        }
    }
}

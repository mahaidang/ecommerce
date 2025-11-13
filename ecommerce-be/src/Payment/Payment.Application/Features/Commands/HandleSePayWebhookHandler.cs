using MediatR;
using Payment.Application.Abstractions.Persistence;
using Payment.Application.Features.Dtos;
using Payment.Domain.Entities;
using MassTransit;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;


namespace Payment.Application.Features.Commands;


public record HandleSePayWebhookCommand(SePayWebhookDto Payload) : IRequest;

public class HandleSePayWebhookHandler : IRequestHandler<HandleSePayWebhookCommand>
{
    private readonly IPaymentRepository _repo;
    private readonly IPublishEndpoint _publisher;

    public HandleSePayWebhookHandler(IPaymentRepository repo, IPublishEndpoint publisher)
    {
        _publisher = publisher;
        _repo = repo;
    }
    public async Task Handle(HandleSePayWebhookCommand cmd, CancellationToken ct)
    {
        var payload = cmd.Payload;

        // 🔍 tìm payment khớp với nội dung chuyển khoản
        var content = payload.Content ?? payload.Code ?? payload.ReferenceCode;
        if (string.IsNullOrWhiteSpace(content))
        {
            await PublishFail(Guid.Empty, content, "Sepay", "PaymentRecordNotFound");
            return;
        }

        var payment = await _repo.FindByContentAsync(content);
        if (payment == null)
        {
            await PublishFail(Guid.Empty, content, "Sepay", "PaymentRecordNotFound");
            return;
        }

        var evt = new EventEnvelope<PaymentSucceededData>(
            Rk.PaymentSucceeded,
            Guid.NewGuid(),
            payment.OrderId,
            payment.OrderNo,
            new PaymentSucceededData("SePay", payment.OrderId.ToString(), payment.Amount),
            DateTime.UtcNow
        );
        await _publisher.Publish(evt);

        payment.Status = "Completed";
        payment.UpdatedAtUtc = DateTime.UtcNow;
        await _repo.UpdateAsync(payment, ct);

        await _repo.AddEventAsync(new PaymentEvent
        {
            Id = Guid.NewGuid(),
            PaymentId = payment.Id,
            EventType = "PaymentCompleted",
            Data = System.Text.Json.JsonSerializer.Serialize(payload),
            CreatedAtUtc = DateTime.UtcNow
        }, ct);

        return;
    }

    // Helper publish failure
    private Task PublishFail(Guid orderId, string orderNo, string provider, string reason)
    {
        var failedData = new PaymentFailedData(provider, reason);

        return _publisher.Publish(
            new EventEnvelope<PaymentFailedData>(
                EventType: "PaymentFailed",
                CorrelationId: Guid.NewGuid(),
                OrderId: orderId,
                OrderNo: orderNo,
                Data: failedData,
                UtcNow: DateTime.UtcNow
            )
        );
    }
}


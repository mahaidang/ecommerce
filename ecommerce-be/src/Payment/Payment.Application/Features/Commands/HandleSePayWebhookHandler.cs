using Azure.Core;
using MediatR;
using Payment.Application.Abstractions.Persistence;
using Payment.Application.Features.Dtos;
using Payment.Domain.Entities;
using System.Text.Json;

namespace Payment.Application.Features.Commands;


public record HandleSePayWebhookCommand(SePayWebhookDto Payload) : IRequest;

public class HandleSePayWebhookHandler : IRequestHandler<HandleSePayWebhookCommand>
{
    private readonly IPaymentRepository _repo;

    public HandleSePayWebhookHandler(IPaymentRepository repo) => _repo = repo;

    public async Task Handle(HandleSePayWebhookCommand cmd, CancellationToken ct)
    {
        var payload = cmd.Payload;

        // 🔍 tìm payment khớp với nội dung chuyển khoản
        var content = payload.Content ?? payload.Code ?? payload.ReferenceCode;
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        var payment = await _repo.FindByContentAsync(content, ct);
        if (payment == null)
        {
            return;
        }

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
}


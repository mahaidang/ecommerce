using MediatR;
using Microsoft.Extensions.Configuration;
using Payment.Application.Abstractions.External;
using Payment.Application.Abstractions.Persistence;
using Payment.Application.Features.Dtos;
using PaymentModel = Payment.Domain.Entities;
using System.Text.Json;

namespace Payment.Application.Features.Commands;

public record CreateSePayPaymentCommand(Guid OrderId, decimal Amount) : IRequest<SePayPaymentResponse>;

public class CreateSePayPaymentHandler : IRequestHandler<CreateSePayPaymentCommand, SePayPaymentResponse>
{
    private readonly IPaymentRepository _repo;
    private readonly ISePayApi _sepay;
    private readonly IConfiguration _config;

    public CreateSePayPaymentHandler(IPaymentRepository repo, ISePayApi sepay, IConfiguration config)
    {
        _repo = repo;
        _sepay = sepay;
        _config = config;
    }

    public async Task<SePayPaymentResponse> Handle(CreateSePayPaymentCommand cmd, CancellationToken ct)
    {
        // 1. Tạo bản ghi Payment pending
        var payment = new PaymentModel.Payment
        {
            Id = Guid.NewGuid(),
            OrderId = cmd.OrderId,
            Amount = cmd.Amount,
            Currency = "VND",
            Status = "Pending",
            Provider = "SePay",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _repo.AddAsync(payment, ct);

        // 2️⃣ Gọi SePayService để tạo QR link
        var req = new SePayPaymentRequest
        {
            OrderCode = $"DH{cmd.OrderId.ToString()[..8]}",
            Amount = cmd.Amount,
            Description = $"Thanh toán đơn hàng {cmd.OrderId}"
        };

        var res = await _sepay.CreatePaymentAsync(req, ct);

        // 3. Lưu link/QR vào ClientSecret
        payment.ClientSecret = res.PaymentUrl;
        await _repo.UpdateAsync(payment, ct);

        // 4. Lưu sự kiện
        await _repo.AddEventAsync(new PaymentModel.PaymentEvent
        {
            Id = Guid.NewGuid(),
            PaymentId = payment.Id,
            EventType = "PaymentCreated",
            Data = JsonSerializer.Serialize(res),
            CreatedAtUtc = DateTime.UtcNow
        }, ct);

        return res;
    }
}
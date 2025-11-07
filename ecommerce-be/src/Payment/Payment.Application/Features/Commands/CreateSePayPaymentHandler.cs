using MediatR;
using Microsoft.Extensions.Configuration;
using Payment.Application.Abstractions.External;
using Payment.Application.Features.Dtos;

namespace Payment.Application.Features.Commands;

public record CreateSePayPaymentCommand(Guid OrderId) : IRequest<SePayPaymentResponse>;

public class CreateSePayPaymentHandler : IRequestHandler<CreateSePayPaymentCommand, SePayPaymentResponse>
{
    private readonly IPay _repo;
    private readonly ISePayApi _sepay;
    private readonly IConfiguration _config;

    public CreateSePayPaymentHandler(IOrderRepository repo, ISePayApi sepay, IConfiguration config)
    {
        _repo = repo;
        _sepay = sepay;
        _config = config;
    }

    public async Task<SePayPaymentResponse> Handle(CreateSePayPaymentCommand cmd, CancellationToken ct)
    {
        var order = await _repo.GetAsync(cmd.OrderId, ct)
            ?? throw new Exception("Order not found");

        var req = new SePayPaymentRequest
        {
            OrderCode = order.Code,
            Amount = order.TotalPrice,
            Description = $"Thanh toán đơn hàng {order.Code}",
            ReturnUrl = _config["SePay:ReturnUrl"],
            CancelUrl = _config["SePay:CancelUrl"]
        };

        var res = await _sepay.CreatePaymentAsync(req, ct);

        order.Status = OrderStatus.WaitingForPayment;
        await _repo.UpdateAsync(order, ct);

        return res;
    }
}
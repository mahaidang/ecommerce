namespace Payment.Application.Features.Dtos;

public class SePayPaymentRequest
{
    public string OrderCode { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public string ReturnUrl { get; set; } = default!;
    public string CancelUrl { get; set; } = default!;
}

public class SePayPaymentResponse
{
    public string QrUrl { get; set; } = default!;
    public string PaymentUrl { get; set; } = default!;
    public string OrderCode { get; set; } = default!;
}

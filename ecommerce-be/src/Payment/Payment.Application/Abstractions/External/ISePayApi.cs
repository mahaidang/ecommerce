using Payment.Application.Features.Dtos;
namespace Payment.Application.Abstractions.External;

public interface ISePayApi
{
    Task<SePayPaymentResponse> CreatePaymentAsync(SePayPaymentRequest req, CancellationToken ct);

}

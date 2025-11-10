using PaymentModel = Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Persistence;

public interface IPaymentRepository
{
    Task AddAsync(PaymentModel.Payment payment, CancellationToken ct);
    Task UpdateAsync(PaymentModel.Payment payment, CancellationToken ct);
    Task<PaymentModel.Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
    Task AddEventAsync(PaymentModel.PaymentEvent evt, CancellationToken ct);
    Task<PaymentModel.Payment?> FindByContentAsync(string content, CancellationToken ct);
}

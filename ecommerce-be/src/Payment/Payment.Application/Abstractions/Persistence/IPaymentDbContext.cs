
using Microsoft.EntityFrameworkCore;
using PaymentModel = Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Persistence;


public interface IPaymentDbContext
{
    DbSet<PaymentModel.Payment> Payments { get; }
    DbSet<PaymentModel.PaymentEvent> PaymentEvents { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

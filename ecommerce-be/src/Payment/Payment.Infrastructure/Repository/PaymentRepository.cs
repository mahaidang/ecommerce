using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Persistence;
using Payment.Infrastructure.Models;
using PaymentModel = Payment.Domain.Entities;

namespace Payment.Infrastructure.Repository;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _db;

    public PaymentRepository(PaymentDbContext db)
    {
        _db = db;
    }


    public async Task AddAsync(PaymentModel.Payment payment, CancellationToken ct)
    {
        await _db.Payments.AddAsync(payment, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PaymentModel.Payment payment, CancellationToken ct)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<PaymentModel.Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
    {
        return await _db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
    }


    public async Task AddEventAsync(PaymentModel.PaymentEvent evt, CancellationToken ct)
    {
        await _db.PaymentEvents.AddAsync(evt, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<PaymentModel.Payment?> FindByContentAsync(string content, CancellationToken ct)
    {
        return await _db.Payments
            .FirstOrDefaultAsync(p =>
                p.ClientSecret.Contains(content) ||
                p.OrderId.ToString().Contains(content), ct);
    }
}

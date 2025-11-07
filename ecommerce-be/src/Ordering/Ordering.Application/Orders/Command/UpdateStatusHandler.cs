using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common;
using Ordering.Domain.Entities;
using System.Text.Json;

namespace Ordering.Application.Orders.Command;

public sealed class UpdateStatusHandler : IRequestHandler<UpdateStatusCommand, bool>
{
    private readonly IOrderingDbContext _db;
    public UpdateStatusHandler(IOrderingDbContext db) => _db = db;

    // Cho phép chuyển tiếp theo “bậc thang”
    private static readonly Dictionary<string, HashSet<string>> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Pending"] = new(StringComparer.OrdinalIgnoreCase) { "Confirmed", "Cancelled", "Paid" },
        ["Confirmed"] = new(StringComparer.OrdinalIgnoreCase) { "Shipped", "Cancelled" },
        ["Paid"] = new(StringComparer.OrdinalIgnoreCase) { "Shipped", "Cancelled" },
        ["Shipped"] = new(StringComparer.OrdinalIgnoreCase) { "Completed" },
        ["Completed"] = new(StringComparer.OrdinalIgnoreCase) { },
        ["Cancelled"] = new(StringComparer.OrdinalIgnoreCase) { }
    };

    public async Task<bool> Handle(UpdateStatusCommand req, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == req.OrderId, ct);
        if (order is null) return false;

        var from = order.Status;
        var to = req.NewStatus.Trim();

        if (!Allowed.TryGetValue(from, out var next) || !next.Contains(to)) return false;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        order.Status = to;
        var now = DateTime.UtcNow;
        order.UpdatedAtUtc = now;

        // Outbox event
        var evt = new { orderId = order.Id, orderNo = order.OrderNo, from, to };
        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = "OrderStatusUpdated",
            Payload = JsonSerializer.Serialize(evt),
            OccurredAtUtc = now
        });

        var ok = await _db.SaveChangesAsync(ct) > 0;
        await tx.CommitAsync(ct);
        return ok;
    }
}

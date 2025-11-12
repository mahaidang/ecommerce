using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Inventory.Application.Features.Commands;

public record ReserveStockCommand(
    Guid OrderId, 
    List<ReservedItem> Items, 
    Guid? CorrelationId,
    string? EventType,
    DateTime? UtcNow,
    bool Pay) : IRequest<bool>;

public class ReserveStockHandler : IRequestHandler<ReserveStockCommand, bool>
{
    private readonly IInventoryDbContext _db;
    private readonly IPublishEndpoint _publisher;

    public ReserveStockHandler(IInventoryDbContext db, IPublishEndpoint publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<bool> Handle(ReserveStockCommand req, CancellationToken ct)
    {
        foreach (var item in req.Items)
        {
            var stock = await _db.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == item.ProductId, ct);

            if (stock == null || stock.Quantity - stock.ReservedQty < item.Quantity)
            {
                var fail = new EventEnvelope<InventoryFailedData>(
                    Rk.InventoryStockFailed,
                    req.CorrelationId ?? Guid.NewGuid(),
                    req.OrderId,
                    new InventoryFailedData("Out of Stock"),
                    DateTime.UtcNow,
                    req.Pay
                );
                await _publisher.Publish(fail);
                return false;
            }
        }

        foreach (var item in req.Items)
        {
            var stock = await _db.Stocks.FirstAsync(s => s.ProductId == item.ProductId, ct);
            stock.ReservedQty += item.Quantity;
            stock.UpdatedAtUtc = DateTime.UtcNow;

            _db.StockMovements.Add(new StockMovement
            {
                ProductId = item.ProductId,
                WarehouseId = stock.WarehouseId,
                ChangeQty = -item.Quantity,
                Reason = "Reserve",
                RefType = "Order",
                RefId = req.OrderId
            });
        }

        await _db.SaveChangesAsync(ct);

        var reserve = new EventEnvelope<InventoryReservedData>(
            Rk.InventoryStockReserved,
            req.CorrelationId ?? Guid.NewGuid(),
            req.OrderId,
            new InventoryReservedData(req.Items),
            DateTime.UtcNow
        );
        await _publisher.Publish(reserve);
        Console.WriteLine("Saga -> Inventory");

        return true;
    }
}

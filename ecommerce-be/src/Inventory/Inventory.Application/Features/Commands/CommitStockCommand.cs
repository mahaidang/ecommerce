using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;

namespace Inventory.Application.Features.Commands;

public record CommitStockCommand(Guid OrderId, List<ItemData> items) : IRequest;

public class CommitStockHandler : IRequestHandler<CommitStockCommand>
{
    private readonly IInventoryDbContext _db;
    private readonly IPublishEndpoint _publisher;

    public CommitStockHandler(IInventoryDbContext db, IPublishEndpoint publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task Handle(CommitStockCommand req, CancellationToken ct)
    {
        foreach (var item in req.items)
        {
            var stock = _db.Stocks
                .FirstOrDefault(s => s.ProductId == item.ProductId);
            if (stock != null)
            {
                stock.ReservedQty -= item.Quantity;
                stock.Quantity -= item.Quantity;
                stock.UpdatedAtUtc = DateTime.UtcNow;

                // Ghi lại movement “Commit”
                _db.StockMovements.Add(new StockMovement
                {
                    ProductId = stock.ProductId,
                    WarehouseId = stock.WarehouseId,
                    ChangeQty = -item.Quantity,
                    Reason = "Commit",
                    RefType = "Order",
                    RefId = req.OrderId
                });
            }

        }
        await _db.SaveChangesAsync(ct);
    }
}
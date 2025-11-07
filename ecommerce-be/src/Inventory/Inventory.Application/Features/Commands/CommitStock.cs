using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Features.Commands;

public record CommitStockCommand(Guid OrderId) : IRequest;

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
        var reservedMovements = await _db.StockMovements
            .Where(sm => sm.RefId == req.OrderId && sm.ChangeQty < 0)
            .ToListAsync(ct);

        foreach (var move in reservedMovements)
        {
            var stock = await _db.Stocks.FirstAsync(s => s.ProductId == move.ProductId, ct);
            stock.ReservedQty -= Math.Abs(move.ChangeQty);
            stock.Quantity -= Math.Abs(move.ChangeQty);     // trừ tồn thật

            // Ghi lại movement “Commit”
            _db.StockMovements.Add(new StockMovement
            {
                ProductId = move.ProductId,
                WarehouseId = move.WarehouseId,
                ChangeQty = move.ChangeQty, // trừ tiếp (âm)
                Reason = "Commit",
                RefType = "Order",
                RefId = req.OrderId
            });
        }

        await _db.SaveChangesAsync(ct);
    }
}
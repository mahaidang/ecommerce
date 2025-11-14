using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Features.Commands;

public record RestockCommandCommand(Guid ProductId, Guid WarehouseId, int Quantity) : IRequest;

public class RestockHandler : IRequestHandler<RestockCommandCommand>
{
    private readonly IInventoryDbContext _db;
    public RestockHandler(IInventoryDbContext db) => _db = db;

    public async Task Handle(RestockCommandCommand cmd, CancellationToken ct)
    {
        var stock = await _db.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == cmd.ProductId && s.WarehouseId == cmd.WarehouseId, ct);

        if (stock is null)
        {
            stock = new Stock { ProductId = cmd.ProductId, WarehouseId = cmd.WarehouseId, Quantity = cmd.Quantity };
            _db.Stocks.Add(stock);
        }
        else stock.Quantity += cmd.Quantity;

        _db.StockMovements.Add(new StockMovement
        {
            ProductId = cmd.ProductId,
            WarehouseId = cmd.WarehouseId,
            ChangeQty = cmd.Quantity,
            Reason = "Restock",
            RefType = "System",
            RefId = Guid.NewGuid()
        });

        await _db.SaveChangesAsync(ct);
    }
}

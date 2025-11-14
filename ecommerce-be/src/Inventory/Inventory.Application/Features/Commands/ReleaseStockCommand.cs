using Inventory.Application.Interfaces;
using MassTransit;
using MediatR;
using Shared.Contracts.Events;

namespace Inventory.Application.Features.Commands;

public record ReleaseStockCommand(List<ItemData> items) : IRequest;

public class ReleaseStockHandler : IRequestHandler<ReleaseStockCommand>
{
    private readonly IInventoryDbContext _db;
    private readonly IPublishEndpoint _publisher;

    public ReleaseStockHandler(IInventoryDbContext db, IPublishEndpoint publisher)
    {
        _db = db;
        _publisher = publisher;
    }
    public async Task Handle(ReleaseStockCommand req, CancellationToken ct)
    {
        foreach (var item in req.items)
        {
            var stock = _db.Stocks
                .FirstOrDefault(s => s.ProductId == item.ProductId);
            if (stock != null)
            {
                stock.ReservedQty -= item.Quantity;
                stock.UpdatedAtUtc = DateTime.UtcNow;
            }
        }
        await _db.SaveChangesAsync(ct);
    }
}


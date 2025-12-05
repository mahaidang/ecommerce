using Inventory.Application.Abstractions.Persistence;
using Inventory.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _db;

    public InventoryRepository(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task<long> GetAvailableQuantityAsync(Guid productId, CancellationToken ct = default)
    {
        var result = await _db.Stocks
            .Where(x => x.ProductId == productId)
            .Select(x => x.Quantity - x.ReservedQty)
            .FirstOrDefaultAsync();

        return result;
    }
}

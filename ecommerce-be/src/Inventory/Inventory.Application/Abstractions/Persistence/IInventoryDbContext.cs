using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Abstractions.Persistence;

public interface IInventoryDbContext
{
    DbSet<Warehouse> Warehouses { get; }
    DbSet<Stock> Stocks { get; }
    DbSet<StockMovement> StockMovements { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

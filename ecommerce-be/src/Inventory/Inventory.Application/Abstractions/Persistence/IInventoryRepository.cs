namespace Inventory.Application.Abstractions.Persistence;

public interface IInventoryRepository
{
    Task<long> GetAvailableQuantityAsync(Guid productId, CancellationToken ct);
}
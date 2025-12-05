namespace Product.Application.Abstractions.External;

public interface IInventoryGrpcClient
{
    Task<long> GetAvailableStockAsync(Guid productId, CancellationToken ct = default);
}
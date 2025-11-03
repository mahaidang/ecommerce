namespace Basket.Application.Abstractions.External;

public interface IProductCatalogClient
{
    Task<ProductReadDto?> GetByIdAsync(Guid productId, CancellationToken ct);
}

public sealed record ProductReadDto(Guid id, string sku, string name, decimal price, string currency);
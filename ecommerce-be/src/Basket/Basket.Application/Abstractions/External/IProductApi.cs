using Refit;


namespace Basket.Application.Abstractions.External;

public interface IProductApi
{
    // Lấy danh sách sản phẩm theo IDs
    [Get("/products/{id}")]
    Task<ProductDto?> GetByIdAsync([AliasAs("id")] Guid id, CancellationToken cancellationToken = default);

}

// Dto riêng cho BasketService (tách khỏi Product domain)
public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    string ImageUrl
);

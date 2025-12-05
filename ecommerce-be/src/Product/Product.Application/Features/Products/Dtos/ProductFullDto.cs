namespace Product.Application.Features.Products.Dtos;

public record ProductFullDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    Guid? CategoryId,
    decimal Price,
    string Currency,
    string Description,
    List<string> Variants,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    List<ProductImageDto>? Images,
    long AvailableQty
);

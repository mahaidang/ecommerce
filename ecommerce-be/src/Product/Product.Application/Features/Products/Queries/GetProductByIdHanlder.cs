using MediatR;
using Product.Application.Abstractions.Persistence;
using Mapster;
using Product.Application.Features.Products.Dtos;

namespace Product.Application.Features.Products.Queries;

public class GetProductByIdHanlder : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _prods;

    public GetProductByIdHanlder(IProductRepository prods)
    {
        _prods = prods;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery req, CancellationToken ct)
    {
        var id = req.Id;
        var product = await _prods.GetByIdAsync(id, ct);
        if (product is null)
            throw new InvalidOperationException($"Product with id '{id}' not found.");

        var mainImage = product.Images?
            .Where(m => m.IsMain)
            .Select(m => m.Adapt<ProductImageDto>())
            .FirstOrDefault();

        var res = product.Adapt<ProductDto>() with { MainImage = mainImage };

        return res;
    }
}

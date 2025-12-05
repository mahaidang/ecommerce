using Azure.Core;
using Mapster;
using MediatR;
using Product.Application.Abstractions.External;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Products.Dtos;
using SharpCompress.Common;

namespace Product.Application.Features.Products.Queries;

public sealed record GetFull(Guid Id) : IRequest<ProductFullDto>;

public class GetFullHanlder : IRequestHandler<GetFull, ProductFullDto>
{
    private readonly IProductRepository _prods;
    private readonly IInventoryGrpcClient _inventoryClient;

    public GetFullHanlder(IProductRepository prods, IInventoryGrpcClient inventoryClient)
    {
        _prods = prods;
        _inventoryClient = inventoryClient;
    }

    public async Task<ProductFullDto> Handle(GetFull req, CancellationToken ct)
    {
        var id = req.Id;
        var product = await _prods.GetByIdAsync(id, ct);
        if (product is null)
            throw new InvalidOperationException($"Product with id '{id}' not found.");

        var qty = await _inventoryClient.GetAvailableStockAsync(id, ct);

        var dto = product.Adapt<ProductFullDto>() with
        {
            AvailableQty = qty
        };
        return dto;
    }
}
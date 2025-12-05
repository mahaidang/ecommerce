using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Dtos;
using MediatR;

namespace Inventory.Application.Features.Queries;

public record GetStockQuery(Guid ProductId) : IRequest<StockDto>;

public class GetStockQueryHandler : IRequestHandler<GetStockQuery, StockDto>
{
    private readonly IInventoryRepository _repo;

    public GetStockQueryHandler(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<StockDto> Handle(GetStockQuery request, CancellationToken ct)
    {
        var qty = await _repo.GetAvailableQuantityAsync(request.ProductId, ct);

        return new StockDto
        {
            ProductId = request.ProductId,
            AvailableQty = qty
        };
    }
}
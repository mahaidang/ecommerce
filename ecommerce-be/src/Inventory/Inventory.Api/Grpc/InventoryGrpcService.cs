using Grpc.Core;
using Inventory.Application.Features.Queries;
using InventoryService.Grpc;
using MediatR;

namespace Inventory.Api.Grpc;

public class InventoryGrpcService : InventoryService.Grpc.InventoryService.InventoryServiceBase
{
    private readonly IMediator _mediator;

    public InventoryGrpcService(IMediator mediator) => _mediator = mediator;

    public override async Task<GetStockResponse> GetStock(
        GetStockRequest request,
        ServerCallContext context)
    {
        var productId = Guid.Parse(request.ProductId);

        var result = await _mediator.Send(new GetStockQuery(productId));

        return new GetStockResponse
        {
            ProductId = result.ProductId.ToString(),
            AvailableQty = result.AvailableQty
        };
    }
}

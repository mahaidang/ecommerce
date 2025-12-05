using Product.Application.Abstractions.External;
using InventoryService.Grpc;
using Grpc.Net.Client;

namespace Product.Infrastructure.Grpc;

public class InventoryGrpcClient : IInventoryGrpcClient
{
    private readonly InventoryService.Grpc.InventoryService.InventoryServiceClient _client;

    public InventoryGrpcClient(InventoryService.Grpc.InventoryService.InventoryServiceClient client)
    {
        _client = client;
    }

    public async Task<long> GetAvailableStockAsync(Guid productId, CancellationToken ct = default)
    {
        var req = new GetStockRequest
        {
            ProductId = productId.ToString()
        };

        var reply = await _client.GetStockAsync(req, cancellationToken: ct);
        return reply.AvailableQty;
    }
}
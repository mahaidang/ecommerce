//using InventoryService.Grpc;
//using Ordering.Application.Inventory;

//namespace Ordering.Infrastructure.Inventory;

//public sealed class GrpcInventoryStockClient : IInventoryStockClient
//{
//    private readonly InventoryService.Grpc.Inventory.InventoryClient _client;
//    public GrpcInventoryStockClient(InventoryService.Grpc.Inventory.InventoryClient client) => _client = client;

//    public async Task<(bool ok, int available, string message)> CheckAsync(Guid productId, int quantity, Guid? warehouseId = null, CancellationToken ct = default)
//    {
//        var req = new CheckStockRequest
//        {
//            ProductId = productId.ToString(),
//            Quantity = quantity,
//            WarehouseId = warehouseId?.ToString() ?? ""
//        };
//        var res = await _client.CheckStockAsync(req, cancellationToken: ct);
//        return (res.Available, res.AvailableQty, res.Message);
//    }
//}

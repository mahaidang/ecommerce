using Grpc.Core;
using Inventory.Application.Interfaces;
using InventoryService.Grpc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Services;

public sealed class InventoryGrpcService : InventoryService.Grpc.InventoryService.InventoryServiceBase
{
    private readonly IInventoryDbContext _db;
    public InventoryGrpcService(IInventoryDbContext db) => _db = db;

    public override async Task<GetStockResponse> GetStock(GetStockRequest req, ServerCallContext context)
    {
        // Tính tồn khả dụng = Quantity - ReservedQty
        var query = _db.Stocks.AsNoTracking().Where(s => s.ProductId == Guid.Parse(req.ProductId));

        var availableQty = await query.SumAsync(s => (int?)(s.Quantity - s.ReservedQty)) ?? 0;

        return new GetStockResponse
        {
            Stock = availableQty,
            Available = availableQty > 0
        };
    }
}

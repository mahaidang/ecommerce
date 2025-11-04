using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common;

namespace Ordering.Application.Orders;

public sealed class GetOrdersByUserHandler : IRequestHandler<GetOrdersByUserQuery, PagedOrdersResult>
{
    private readonly IOrderingDbContext _db;
    public GetOrdersByUserHandler(IOrderingDbContext db) => _db = db;

    public async Task<PagedOrdersResult> Handle(GetOrdersByUserQuery req, CancellationToken ct)
    {
        var page = Math.Max(1, req.Page);
        var pageSize = Math.Clamp(req.PageSize, 1, 200);

        var q = _db.Orders
                   .AsNoTracking()
                   .Where(x => (x.UserId == req.UserId && (string.IsNullOrEmpty(req.status) || x.Status == req.status)))
                   .OrderByDescending(x => x.CreatedAtUtc);

        var total = await q.CountAsync(ct);
        var items = await q
                        .Where(c => string.IsNullOrEmpty(req.status) || c.Status == req.status)
                        .OrderByDescending(c => c.CreatedAtUtc) // nếu cần sắp xếp
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(x => new OrderListItemDto(
                            x.Id, x.OrderNo, x.UserId, x.Status, x.Currency,
                            x.GrandTotal, x.CreatedAtUtc))
                        .ToListAsync(ct);


        return new PagedOrdersResult(total, page, pageSize, items);
    }
}

public sealed class GetOrderDetailHandler : IRequestHandler<GetOrderDetailQuery, OrderDetailDto?>
{
    private readonly IOrderingDbContext _db;
    public GetOrderDetailHandler(IOrderingDbContext db) => _db = db;

    public async Task<OrderDetailDto?> Handle(GetOrderDetailQuery req, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Where(o => o.Id == req.OrderId)
            .Select(o => new OrderDetailDto(
                o.Id, o.OrderNo, o.UserId, o.Status, o.Currency,
                o.Subtotal, o.DiscountTotal, o.ShippingFee, o.GrandTotal,
                o.Note, o.CreatedAtUtc,
                o.OrderItems.Select(i => new OrderItemDto(
                    i.Id, i.ProductId, i.Sku, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return order;
    }
}

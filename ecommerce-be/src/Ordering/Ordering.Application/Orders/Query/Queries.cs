using MediatR;

namespace Ordering.Application.Orders;

// GET /api/Orders?userId=...&page=&pageSize=
public record GetOrdersByUserQuery(Guid UserId, int Page = 1, int PageSize = 20, string status = "")
  : IRequest<PagedOrdersResult>;

public record PagedOrdersResult(int Total, int Page, int PageSize, IReadOnlyList<OrderListItemDto> Items);

public record OrderListItemDto(
    Guid Id, string OrderNo, Guid UserId, string Status, string Currency,
    decimal? GrandTotal, DateTime CreatedAtUtc);

// GET /api/Orders/{id}
public record GetOrderDetailQuery(Guid OrderId) : IRequest<OrderDetailDto?>;

public record OrderItemDto(Guid Id, Guid ProductId, string Sku, string ProductName, decimal UnitPrice, int Quantity, decimal? LineTotal);
public record OrderDetailDto(
    Guid Id, string OrderNo, Guid UserId, string Status, string Currency,
    decimal Subtotal, decimal DiscountTotal, decimal ShippingFee, decimal? GrandTotal,
    string? Note, DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemDto> Items);

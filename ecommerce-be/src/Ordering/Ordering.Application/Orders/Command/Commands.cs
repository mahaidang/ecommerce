using MediatR;

namespace Ordering.Application.Orders.Command;

public record CreateOrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);
public record CreateOrderCommand(
    Guid UserId, decimal DiscountTotal, decimal ShippingFee, string? Note,
    IReadOnlyList<CreateOrderItemDto> Items
) : IRequest<CreateOrderResult>;
public record CreateOrderResult(Guid OrderId, string OrderNo, decimal? GrandTotal);

public record CancelOrderCommand(Guid OrderId, string? Reason) : IRequest<bool>;

public record UpdateStatusCommand(Guid OrderId, string NewStatus) : IRequest<bool>;

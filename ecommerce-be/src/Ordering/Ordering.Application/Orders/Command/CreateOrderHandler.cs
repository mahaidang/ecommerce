using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Common;
using Ordering.Domain.Entities;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;


namespace Ordering.Application.Orders.Command;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderingDbContext _db;
    private readonly IPublishEndpoint _publisher;
    private readonly ILogger<CreateOrderHandler> _log;

    public CreateOrderHandler(IOrderingDbContext db, IPublishEndpoint publisher, ILogger<CreateOrderHandler> log)
    {
        _db = db;
        _publisher = publisher;
        _log = log;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand req, CancellationToken ct)
    {
        _log.LogError(req.Pay.ToString());
        if (req.Items is null || req.Items.Count == 0) 
            throw new ArgumentException("Items empty");

        var subtotal = req.Items.Sum(i => i.UnitPrice * i.Quantity); 
        var grand = subtotal - req.DiscountTotal + req.ShippingFee;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            OrderNo = GenOrderNo(),
            Status = "Pending",
            Subtotal = subtotal,
            DiscountTotal = req.DiscountTotal,
            ShippingFee = req.ShippingFee,
            GrandTotal = grand,
            Note = req.Note,
            FullAddress = req.Address.Trim(),
            Name = req.Name.Trim(),
            PhoneNumber = req.Phone.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        foreach (var i in req.Items)
        {
            var o = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName.Trim(),
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                LineTotal = i.UnitPrice * i.Quantity
            };
            order.OrderItems.Add(o);
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        // 🟢 Wrap message trong EventEnvelope cho Orchestrator
        var orderCreatedData = new OrderCreatedData(
            order.UserId,
            "VND", // currency
            order.GrandTotal ?? 0,
            order.OrderItems.Select(x =>
                new OrderItemData(x.ProductId, x.ProductName, x.Quantity, x.UnitPrice)
            ).ToList()
        );

        var envelope = new EventEnvelope<OrderCreatedData>(
            Rk.OrderCreated,
            Guid.NewGuid(), // correlationId
            order.Id,
            order.OrderNo,
            orderCreatedData,
            DateTime.UtcNow,
            req.Pay
        );

        await _publisher.Publish(envelope, ct);
        _log.LogError("Order -> Saga");

        return new CreateOrderResult(order.Id, order.OrderNo, order.GrandTotal);
    }

    private static string GenOrderNo()
    {
        var rand = Random.Shared.Next(10000, 99999);
        return $"ORD{DateTime.UtcNow:yyyyMMdd}{rand}";
    }
}
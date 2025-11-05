using MediatR;
using Ordering.Application.Common;
using Ordering.Domain.Entities;
using System.Text.Json;
using MassTransit;


namespace Ordering.Application.Orders.Command;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderingDbContext _db;
    private readonly IPublishEndpoint _publisher;

    public CreateOrderHandler(IOrderingDbContext db, IPublishEndpoint publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand req, CancellationToken ct)
    {
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
            "order.created",
            Guid.NewGuid(), // correlationId
            order.Id,
            orderCreatedData,
            DateTime.UtcNow
        );

        await _publisher.Publish(envelope, ct);


        return new CreateOrderResult(order.Id, order.OrderNo, order.GrandTotal);
    }

    private static string GenOrderNo()
    {
        var rand = Random.Shared.Next(1000, 9999);
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{rand}";
    }
}

public record OrderItemData(Guid ProductId, string Name, int Quantity, decimal UnitPrice);
public record OrderCreatedData(Guid UserId, string Currency, decimal GrandTotal,
    IReadOnlyList<OrderItemData> Items); 
public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime utcNow);

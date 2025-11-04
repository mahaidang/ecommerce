using MediatR;
using Ordering.Application.Common;
using Ordering.Domain.Entities;
using System.Text.Json;

namespace Ordering.Application.Orders.Command;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderingDbContext _db;
    public CreateOrderHandler(IOrderingDbContext db) => _db = db;

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

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        _db.Orders.Add(order);

        // Build event data using local DTOs (decoupled from Orchestrator service)
        var orderCreatedData = new OrderCreatedData(
            order.UserId,
            order.Currency,
            order.GrandTotal ?? 0,
            order.OrderItems.Select(x =>
                new OrderItemData(
                    x.ProductId,
                    x.Sku,
                    x.ProductName,
                    x.Quantity,
                    x.UnitPrice,
                    x.LineTotal
                )
            ).ToList()
        );

        var envelope = new EventEnvelope<OrderCreatedData>(
            "order.created",
            Guid.NewGuid(), // correlationId
            order.Id,
            orderCreatedData,
            DateTime.UtcNow
        );

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = "order.created",
            Payload = JsonSerializer.Serialize(envelope, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            OccurredAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new CreateOrderResult(order.Id, order.OrderNo, order.GrandTotal);
    }

    private static string GenOrderNo()
    {
        var rand = Random.Shared.Next(1000, 9999);
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{rand}";
    }
}

// Local DTOs/envelope to avoid external dependency
public sealed record OrderItemData(Guid ProductId, string Sku, string ProductName, int Quantity, decimal UnitPrice, decimal? LineTotal);
public sealed record OrderCreatedData(Guid UserId, string Currency, decimal GrandTotal, List<OrderItemData> Items);
public sealed record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid AggregateId, T Data, DateTime OccurredAtUtc);

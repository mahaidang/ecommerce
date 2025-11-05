namespace OrchestratorService.Worker.Messaging;

// Envelope chung
public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime utcNow);

// Events
public record OrderCreatedData(Guid UserId, string Currency, decimal GrandTotal,
    IReadOnlyList<OrderItemData> Items);
public record OrderItemData(Guid ProductId, string Name, int Quantity, decimal UnitPrice);

// Inventory
public record InventoryReservedData(Guid WarehouseId, IReadOnlyList<ReservedItem> Items);
public record ReservedItem(Guid ProductId, int Quantity);
public record InventoryFailedData(string Reason);

// Payment
public record PaymentSucceededData(string Provider, string TxnRef, decimal Amount, string Currency);
public record PaymentFailedData(string Provider, string Reason);

// Commands (payload)
public record CmdInventoryReserve(Guid OrderId, IReadOnlyList<ReservedItem> Items);
public record CmdInventoryRelease(Guid OrderId, IReadOnlyList<ReservedItem> Items);
public record CmdPaymentRequest(Guid OrderId, decimal Amount, string Currency);
public record CmdPaymentCancel(Guid OrderId, string Reason);
public record CmdOrderUpdateStatus(Guid OrderId, string NewStatus);

// OrderFailed
public record OrderFailedData(string Reason);

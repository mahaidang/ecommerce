namespace Shared.Contracts.Events;

public record EventEnvelope<T>(
    string EventType,
    Guid CorrelationId,
    Guid OrderId,
    T Data,
    DateTime UtcNow,
    bool Pay = false
);

public record OrderItemData(Guid ProductId, string Name, int Quantity, decimal UnitPrice);

public record OrderCreatedData(Guid UserId, string Currency, decimal GrandTotal, IReadOnlyList<OrderItemData> Items );

public record CmdInventoryReserve(Guid OrderId, IReadOnlyList<ReservedItem> Items);
public record CmdInventoryRelease(Guid OrderId, IReadOnlyList<ReservedItem> Items);


// Inventory
public record InventoryReservedData(IReadOnlyList<ReservedItem> Items);
public record ReservedItem(Guid ProductId, int Quantity);
public record InventoryFailedData(string Reason);

public record CmdPaymentRequest(Guid OrderId, decimal Amount, string Currency);
public record CmdPaymentCancel(Guid OrderId, string Reason);
public record CmdOrderUpdateStatus(Guid OrderId, string NewStatus);

// OrderFailed
public record OrderFailed(string Reason);
public record OrderSucceeded(string Reason);


// Payment
public record PaymentSucceededData(string Provider, string TxnRef, decimal Amount);
public record PaymentFailedData(string Provider, string Reason);

public record   OrderApprovalResult(bool Approved, string? Note = null);

//commit
public record CmdInventoryCommit;
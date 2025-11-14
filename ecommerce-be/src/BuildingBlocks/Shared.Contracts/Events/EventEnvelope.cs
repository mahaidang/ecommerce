namespace Shared.Contracts.Events;

// Event Envelope (wrapper)
public record EventEnvelope<T>(
    string EventType,
    Guid CorrelationId,
    Guid OrderId,
    string OrderNo,
    T Data,
    DateTime UtcNow,
    bool Pay = false
);

// ITEM DATA
public record ItemData(Guid ProductId, int Quantity);

// 1) ORDER EVENTS

public record OrderCreatedData(Guid UserId, string Currency, decimal GrandTotal, IReadOnlyList<ItemData> Items );

// 2) INVENTORY COMMANDS (Saga -> Inventory)
public record CmdInventoryReserve(IReadOnlyList<ItemData> Items);
public record CmdInventoryRelease(IReadOnlyList<ItemData> Items);
public record CmdInventoryCommit(IReadOnlyList<ItemData> Items);

// 3) INVENTORY EVENTS (Inventory -> Saga)
public record InventoryReservedData(IReadOnlyList<ItemData> Items);
public record InventoryFailedData(string Reason);

// 4) PAYMENT COMMANDS (Saga -> Payment)
public record CmdPaymentRequest(decimal Amount, string Currency);
public record CmdPaymentCancel(string Reason);

// 5) PAYMENT EVENTS (Payment -> Saga)
public record PaymentSucceededData(string Provider, string TxnRef, decimal Amount);
public record PaymentFailedData(string Provider, string Reason);

// 6) ORDER COMMANDS (Saga -> Order)
public record CmdOrderUpdateStatus(string NewStatus);

// 7) ADMIN EVENTS (Identity -> Saga)
public record OrderApprovalResult(bool Approved, string? Note = null);
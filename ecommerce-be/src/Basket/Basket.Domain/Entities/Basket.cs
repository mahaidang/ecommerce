namespace Basket.Domain.Entities;

public class Basket
{
    // UserId có thể null nếu là guest
    public Guid? UserId { get; set; }

    // SessionId dành cho khách vãng lai
    public string? SessionId { get; set; }

    public List<BasketItem> Items { get; set; } = new();
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class BasketItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

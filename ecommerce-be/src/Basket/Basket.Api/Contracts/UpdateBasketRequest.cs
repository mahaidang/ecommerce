namespace Basket.Api.Contracts;

public class UpdateBasketRequest
{
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }

    public List<UpdateBasketItemRequest> Items { get; set; } = new();
}

public class UpdateBasketItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

namespace Basket.Application.Features.Baskets.Dtos;

public class UpdateBasketDto
{
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }

    public List<UpdateBasketItemDto> Items { get; set; } = new();
}

public class UpdateBasketItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

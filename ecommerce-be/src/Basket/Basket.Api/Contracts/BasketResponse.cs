namespace Basket.Api.Contracts;

public sealed record BasketResponse(Guid UserId, List<BasketItemResponse> Items);
public sealed record BasketItemResponse(Guid ProductId, int Quantity);

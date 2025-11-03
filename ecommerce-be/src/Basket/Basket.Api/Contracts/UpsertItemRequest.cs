namespace Basket.Api.Contracts;

public sealed record SaveItemRequest(Guid ProductId, int Quantity);
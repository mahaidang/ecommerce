namespace Basket.Application.Features.Baskets.Dtos;

public record SaveBasketDto(Guid? UserId, String? SessionId, Guid ProductId, int Quantity);

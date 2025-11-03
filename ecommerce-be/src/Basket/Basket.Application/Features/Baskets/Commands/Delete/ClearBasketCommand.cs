using MediatR;

namespace Basket.Application.Features.Baskets.Commands.Delete;

public sealed record ClearBasketCommand(Guid UserId) : IRequest<Unit>;

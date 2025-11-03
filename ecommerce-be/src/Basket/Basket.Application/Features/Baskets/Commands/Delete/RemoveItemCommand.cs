using MediatR;

namespace Basket.Application.Features.Baskets.Commands.Delete;

public sealed record RemoveItemCommand(Guid UserId, Guid ProductId, TimeSpan? Ttl) : IRequest<Unit>;

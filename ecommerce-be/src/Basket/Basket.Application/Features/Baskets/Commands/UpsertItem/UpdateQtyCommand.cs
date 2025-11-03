using MediatR;

namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

public sealed record UpdateQtyCommand(Guid UserId, Guid ProductId, int Quantity, TimeSpan? Ttl)
    : IRequest<Domain.Entities.Basket>;

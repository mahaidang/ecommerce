using Basket.Domain.Entities;
using MediatR;

namespace Basket.Application.Features.Baskets.Queries;

public sealed record GetEnrichedBasketQuery(Guid UserId) : IRequest<Domain.Entities.Basket>;

using Basket.Application.Abstractions.Persistence;
using MediatR;

namespace Basket.Application.Features.Baskets.Commands.Delete;

public sealed class ClearBasketHandler : IRequestHandler<ClearBasketCommand, Unit>
{
    private readonly IBasketRepository _repo;

    public ClearBasketHandler(IBasketRepository repo) => _repo = repo;
    public async Task<Unit> Handle(ClearBasketCommand c, CancellationToken ct)
    {
        await _repo.ClearAsync(c.UserId, ct);
        return Unit.Value;
    }
}
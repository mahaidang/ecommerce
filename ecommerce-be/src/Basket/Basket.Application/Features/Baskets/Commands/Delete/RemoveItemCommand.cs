using Basket.Application.Abstractions.Persistence;
using MediatR;

namespace Basket.Application.Features.Baskets.Commands.Delete;

public sealed record RemoveItemCommand(Guid? userId, string? sessionId, Guid productId) : IRequest<Unit>;

public class RemoveItemHandler : IRequestHandler<RemoveItemCommand, Unit>
{
    private readonly IBasketRepository _repo;

    public RemoveItemHandler(IBasketRepository repo) => _repo = repo;

    public async Task<Unit> Handle(RemoveItemCommand c, CancellationToken ct)
    {
        string key = "";
        TimeSpan? ttl = null;
        if (c.userId != null && c.userId != Guid.Empty)
        {
            key = $"basket:user:{c.userId}";
            ttl = null; // vĩnh viễn
        }
        else
        {
            key = $"basket:guest:{c.sessionId}";
            ttl = TimeSpan.FromDays(30);
        }
        var ok = await _repo.RemoveItemAsync(key, c.productId, ttl, ct);
        if (!ok) throw new KeyNotFoundException("Item not found");
        return Unit.Value;
    }
}
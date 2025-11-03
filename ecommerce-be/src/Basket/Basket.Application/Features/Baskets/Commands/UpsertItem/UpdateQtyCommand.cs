using Basket.Application.Abstractions.Persistence;
using Basket.Application.Features.Baskets.Dtos;
using MediatR;

namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

public sealed record UpdateQtyCommand(SaveBasketDto dto)
    : IRequest<Domain.Entities.Basket>;

public class UpdateQtyHandler : IRequestHandler<UpdateQtyCommand, Domain.Entities.Basket>
{
    private readonly IBasketRepository _repo;
    public UpdateQtyHandler(IBasketRepository repo) => _repo = repo;
    public async Task<Domain.Entities.Basket> Handle(UpdateQtyCommand c, CancellationToken ct)
    {
        string key;
        TimeSpan? ttl = null;

        if (c.dto.UserId != null && c.dto.UserId != Guid.Empty)
        {
            key = $"basket:user:{c.dto.UserId}";
        }
        else
        {
            key = $"basket:guest:{c.dto.SessionId}";
            ttl = TimeSpan.FromDays(30);
        }


        var basket = await _repo.GetAsync(key, ct) ?? new Domain.Entities.Basket { UserId = c.dto.UserId, SessionId = c.dto.SessionId, Items = new() };
        var it = basket.Items.FirstOrDefault(x => x.ProductId == c.dto.ProductId);
        if (it is null) throw new KeyNotFoundException("Item not found");
        it.Quantity = c.dto.Quantity;
        await _repo.UpsertAsync(key, basket, ttl, ct);
        return await _repo.GetAsync(key, ct) ?? new Domain.Entities.Basket { UserId = c.dto.UserId, SessionId = c.dto.SessionId, Items = new() };
    }
}
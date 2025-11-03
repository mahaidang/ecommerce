using Basket.Application.Features.Baskets.Dtos;
using Basket.Application.Abstractions.Persistence;
using MediatR;

namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

public sealed record SaveItemCommand(SaveBasketDto dto) : IRequest<Domain.Entities.Basket>;

public class SaveItemHandler : IRequestHandler<SaveItemCommand, Domain.Entities.Basket>
{
    private readonly IBasketRepository _repo;
    public SaveItemHandler(IBasketRepository repo) => _repo = repo;
    public async Task<Domain.Entities.Basket> Handle(SaveItemCommand c, CancellationToken ct)
    {
        string key;
        TimeSpan? ttl = null;

        if (c.dto.UserId != null && c.dto.UserId != Guid.Empty)
        {
            key = $"basket:user:{c.dto.UserId}";
            ttl = null; // vĩnh viễn
        }
        else
        {
            key = $"basket:guest:{c.dto.SessionId}";
            ttl = TimeSpan.FromDays(30);
        }


        await _repo.SaveBasketAsync(key, c.dto, ct, ttl);
        return await _repo.GetAsync(key, ct) ?? new Domain.Entities.Basket { UserId = c.dto.UserId, SessionId = c.dto.SessionId, Items = new() };
    }
}
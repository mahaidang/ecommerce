using Basket.Application.Features.Baskets.Dtos;

namespace Basket.Application.Abstractions.Persistence;

public interface IBasketRepository
{
    Task<Domain.Entities.Basket?> GetAsync(string key, CancellationToken ct);
    Task UpsertAsync(string key, Domain.Entities.Basket basket, TimeSpan? ttl, CancellationToken ct);
    //Task<bool> RemoveItemAsync(Guid userId, Guid productId, TimeSpan? ttl, CancellationToken ct);
    //Task AddOrUpdateItemAsync(Guid userId, BasketItem item, TimeSpan? ttl, CancellationToken ct);
    Task ClearAsync(Guid userId, CancellationToken ct);
    Task SaveBasketAsync(string key, SaveBasketDto dto, CancellationToken ct, TimeSpan? ttl = null);
}

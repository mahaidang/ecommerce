using Basket.Application.Features.Baskets.Dtos;
using Basket.Application.Abstractions.Persistence;
using Basket.Application.Features.Baskets.Queries;
using Basket.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.Infrastructure.Persistence;

public sealed class RedisBasketRepository : IBasketRepository
{
    private readonly IDatabase _db;
    private static string Key(Guid userId) => $"basket:{userId:D}";

    private static readonly JsonSerializerOptions JsonOpt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisBasketRepository(IConnectionMultiplexer mux)
        => _db = mux.GetDatabase();

    public async Task<Domain.Entities.Basket?> GetAsync(string key, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return null;

        ct.ThrowIfCancellationRequested();
        return JsonSerializer.Deserialize<Domain.Entities.Basket>(value!, JsonOpt);
    }

    public async Task UpsertAsync(string key, Domain.Entities.Basket basket, TimeSpan? ttl, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        basket.UpdatedAtUtc = DateTime.UtcNow;
        var payload = JsonSerializer.Serialize(basket, JsonOpt);

        // StackExchange.Redis chưa nhận CancellationToken ở các API phổ biến

        await _db.StringSetAsync(key, payload, ttl);

        ct.ThrowIfCancellationRequested();
    }

    public async Task<bool> RemoveItemAsync(string key, Guid productId, TimeSpan? ttl, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var basket = await GetAsync(key, ct);
        if (basket is null) return false;

        basket.Items ??= new List<BasketItem>();
        var removed = basket.Items.RemoveAll(i => i.ProductId == productId) > 0;
        if (!removed) return false;

        await UpsertAsync(key, basket, ttl, ct);
        return true;
    }

    //clear
    public async Task ClearAsync(Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        await _db.KeyDeleteAsync(Key(userId));
    }

    public async Task SaveBasketAsync(string key, SaveBasketDto dto, CancellationToken ct, TimeSpan? ttl = null)
    {
        ct.ThrowIfCancellationRequested();

        var basket = await GetAsync(key, ct) ?? new Domain.Entities.Basket { UserId = dto.UserId, SessionId = dto.SessionId, Items = new() };

        var existing = basket.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (existing is null)
        {
            basket.Items.Add(new BasketItem { ProductId = dto.ProductId, Quantity = dto.Quantity });
        }
        else
        {
            existing.Quantity = dto.Quantity;
        }

        await UpsertAsync(key, basket, ttl, ct);
    }
}

using Basket.Application.Abstractions.External;
using Basket.Application.Abstractions.Persistence;
using Mapster;
using MediatR;

namespace Basket.Application.Features.Baskets.Queries;

public record GetBasketQuery(Guid? userId, string? sessionId) : IRequest<BasketDto>;


public class GetBasketHandler : IRequestHandler<GetBasketQuery, BasketDto>
{
    private readonly IBasketRepository _repo;
    private readonly IProductApi _productApi;

    public GetBasketHandler(IBasketRepository repo, IProductApi productApi)
    {
        _repo = repo;
        _productApi = productApi;
    }

    public async Task<BasketDto> Handle(GetBasketQuery q, CancellationToken ct)
    {
        string key;

        if (q.userId != null && q.userId != Guid.Empty)
        {
            key = $"basket:user:{q.userId}";
        }
        else
        {
            key = $"basket:guest:{q.sessionId}";
        }
        var basket = await _repo.GetAsync(key, ct);
        if (basket == null) return new BasketDto(q.userId, q.sessionId, new());

        var ids = basket.Items.Select(i => i.ProductId);

        var items = new List<BasketItemDto>();

        foreach (var id in ids)
        {
            var prod = await _productApi.GetByIdAsync(id, ct);

            if (prod != null)
            {
                // Ensure MainImage is not null to avoid CS8604
                var image = prod.MainImage ?? new ProductImageDto
                {
                    Url = string.Empty,
                    PublicId = string.Empty,
                    IsMain = true,
                    Alt = null
                };

                items.Add(new BasketItemDto(
                    image,
                    prod.Name,
                    prod.Price,
                    basket.Items.First(i => i.ProductId == id).Quantity,
                    prod.Id
                ));
            }
        }


        return new BasketDto(basket.UserId, basket.SessionId, items);
    }
}

public record BasketItemDto(
    ProductImageDto ImageUrl,
    string Name,
    decimal Price,
    int Quantity,
    Guid Id
);

public record BasketDto(
    Guid? UserId,
    string? SessionId,
    List<BasketItemDto> Items
);
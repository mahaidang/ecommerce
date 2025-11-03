//using Basket.Application.Abstractions.Persistence;
//using Basket.Domain.Entities;
//using MediatR;

//namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

//public sealed class UpsertItemHandler : IRequestHandler<UpsertItemCommand, Domain.Entities.Basket>
//{
//    private readonly IBasketRepository _repo;

//    public UpsertItemHandler(IBasketRepository repo) => _repo = repo;

//    public async Task<Domain.Entities.Basket> Handle(UpsertItemCommand c, CancellationToken ct)
//    {
//        var item = new BasketItem
//        {
//            ProductId = c.ProductId,
//            Quantity = c.Quantity,
//        };

//        await _repo.AddOrUpdateItemAsync(c.UserId, item, c.Ttl, ct);
//        return await _repo.GetAsync(c.UserId, ct) ?? new Domain.Entities.Basket { UserId = c.UserId, Items = new() };
//    }
//}

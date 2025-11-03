//using Basket.Application.Abstractions.Persistence;
//using MediatR;

//namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

//public sealed class UpdateQtyHandler : IRequestHandler<UpdateQtyCommand, Domain.Entities.Basket>
//{
//    private readonly IBasketRepository _repo;

//    public UpdateQtyHandler(IBasketRepository repo) => _repo = repo;

//    public async Task<Domain.Entities.Basket> Handle(UpdateQtyCommand c, CancellationToken ct)
//    {
//        var basket = await _repo.GetAsync(c.UserId, ct) ?? new Domain.Entities.Basket { UserId = c.UserId, Items = new() };
//        var it = basket.Items.FirstOrDefault(x => x.ProductId == c.ProductId);
//        if (it is null) throw new KeyNotFoundException("Item not found");
//        it.Quantity = c.Quantity;
//        await _repo.UpsertAsync(basket, c.Ttl, ct);
//        return basket;
//    }
//}
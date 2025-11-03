//using Basket.Application.Abstractions.Persistence;
//using MediatR;

//namespace Basket.Application.Features.Baskets.Commands.Delete;

//public sealed class RemoveItemHandler : IRequestHandler<RemoveItemCommand, Unit>
//{
//    private readonly IBasketRepository _repo;

//    public RemoveItemHandler(IBasketRepository repo) => _repo = repo;
//    public async Task<Unit> Handle(RemoveItemCommand c, CancellationToken ct)
//    {
//        var ok = await _repo.RemoveItemAsync(c.UserId, c.ProductId, c.Ttl, ct);
//        if (!ok) throw new KeyNotFoundException("Item not found");
//        return Unit.Value;
//    }
//}

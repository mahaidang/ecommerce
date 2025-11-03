using Basket.Application.Abstractions.Persistence;
using Basket.Application.Features.Baskets.Dtos;
using MediatR;

namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

public record UpdateBasketCommand(UpdateBasketDto dto) : IRequest<UpdateBasketDto>;

public class UpdateBasketHandler : IRequestHandler<UpdateBasketCommand, UpdateBasketDto> {
    private readonly IBasketRepository _repo;
    public UpdateBasketHandler(IBasketRepository repo) => _repo = repo;

    public async Task<UpdateBasketDto> Handle(UpdateBasketCommand request, CancellationToken ct)
    {
        var key = request.dto.UserId != null && request.dto.UserId != Guid.Empty
            ? $"basket:user:{request.dto.UserId}"
            : $"basket:guest:{request.dto.SessionId}";
        var basket = await _repo.GetAsync(key, ct) ?? new Domain.Entities.Basket { UserId = request.dto.UserId, SessionId = request.dto.SessionId, Items = new() };

        // Instead of basket.Clear(), clear the Items list directly
        basket.Items.Clear();

        foreach (var itemReq in request.dto.Items)
        {
            basket.Items.Add(new Domain.Entities.BasketItem {
                ProductId = itemReq.ProductId,
                Quantity = itemReq.Quantity
            });
        }

        basket.UpdatedAtUtc = DateTime.UtcNow;

        // TTL: 14 ngày cho guest, vĩnh viễn cho user
        TimeSpan? ttl = (request.dto.UserId != null && request.dto.UserId != Guid.Empty)
            ? null
            : TimeSpan.FromDays(14);

        // Lưu lại giỏ hàng
        await _repo.UpsertAsync(key, basket, ttl, ct);

        // Trả về DTO (nếu cần FE sync lại)
        var result = new UpdateBasketDto
        {
            UserId = basket.UserId,
            SessionId = basket.SessionId,
            Items = basket.Items.Select(i => new UpdateBasketItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        return result;

    }
}

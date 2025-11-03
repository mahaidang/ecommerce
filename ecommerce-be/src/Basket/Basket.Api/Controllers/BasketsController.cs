using Basket.Api.Contracts;
using Basket.Application.Features.Baskets.Commands.Delete;
using Basket.Application.Features.Baskets.Commands.UpsertItem;
using Basket.Application.Features.Baskets.Dtos;
using Basket.Application.Features.Baskets.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class BasketsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly TimeSpan _ttl;

    public BasketsController(ISender sender, IConfiguration cfg)
    {
        _sender = sender;
        var minutes = cfg.GetValue<int?>("Redis:DefaultTtlMinutes") ?? 4320;
        _ttl = TimeSpan.FromMinutes(minutes);
    }

    //get by id
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid? userId,
        [FromQuery] string? sessionId,
        CancellationToken ct)
    {
        var b = await _sender.Send(new GetBasketQuery(userId, sessionId), ct);
        return Ok(b);
    }

    //post items
    [HttpPost]
    public async Task<IActionResult> SaveItem(
        [FromQuery] Guid? userId, 
        [FromQuery] string? sessionId, 
        [FromBody] SaveItemRequest req, 
        CancellationToken ct)
    {
        var b = await _sender.Send(new SaveItemCommand(new SaveBasketDto(userId, sessionId, req.ProductId, req.Quantity)), ct);
        return Ok(b);
    }

    //update quantity
    [HttpPatch]
    public async Task<IActionResult> UpdateQty(
        [FromQuery] Guid? userId,
        [FromQuery] string? sessionId,
        [FromBody] SaveItemRequest req,
        CancellationToken ct)
    {
        var b = await _sender.Send(new UpdateQtyCommand(new SaveBasketDto(userId, sessionId, req.ProductId, req.Quantity)), ct);
        return Ok(b);
    }

    //update basket
    [HttpPut("update-all")]
    public async Task<IActionResult> UpdateAll([FromBody] UpdateBasketRequest request, CancellationToken ct)
    {
        var dto = request.Adapt<UpdateBasketDto>();
        var rs = await _sender.Send(new UpdateBasketCommand(dto), ct);
        return Ok(new { message = "Basket updated successfully" });
    }
    //delete product
    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid userId, Guid productId, CancellationToken ct)
    {
        try
        {
            await _sender.Send(new RemoveItemCommand(userId, productId, _ttl), ct);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    //delete products
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Clear(Guid userId, CancellationToken ct)
    {
        await _sender.Send(new ClearBasketCommand(userId), ct);
        return NoContent();
    }

    //[HttpGet("{userId:guid}/enrich")]
    //public async Task<IActionResult> GetEnriched(Guid userId, CancellationToken ct)
    //{
    //    var b = await _sender.Send(new GetEnrichedBasketQuery(userId), ct);
    //    return Ok(Map(b));
    //}

    //private static BasketResponse Map(Basket.Domain.Entities.Basket b)
    //    => new(b.UserId, b.Items.Select(i =>
    //        new BasketItemResponse(i.ProductId, i.Quantity)).ToList());
}

using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Product.Api.Contracts.CreateProduct;
using Product.Api.Contracts.UpdateProduct;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Products.Commands.CreateProduct;
using Product.Application.Features.Products.Commands.DeleteProduct;
using Product.Application.Features.Products.Commands.UpdateProduct;
using Product.Application.Features.Products.Dtos;
using Product.Application.Features.Products.Queries;

namespace Product.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly ISender _sender;
    public ProductsController(IProductRepository repo, ISender sender)
    {
        _repo = repo;
        _sender = sender;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateRequest req, CancellationToken ct)
    {
        var dto = req.Adapt<CreateProductDto>();
        var cmd = new CreateProductCommand(dto);
        var res = await _sender.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);

    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var p = await _sender.Send(new GetProductByIdQuery(id), ct);
        return p is null ? NotFound() : Ok(p.Adapt<ProductDto>());
    }

    [HttpGet("{id:guid}/full")]
    public async Task<IActionResult> GetFull([FromRoute] Guid id, CancellationToken ct)
    {
        var p = await _sender.Send(new GetFull(id), ct);
        return p is null ? NotFound() : Ok(p.Adapt<ProductFullDto>());
    }

    //Lấy danh sách sản phẩm
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? keyword,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetProductsQuery(keyword, categoryId, minPrice, maxPrice, page, pageSize), ct);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest req, CancellationToken ct)
    {
        var exists = await _repo.ExistsAsync(id, ct);
        if (!exists) return NotFound();


        var dto = req.Adapt<UpdateProductDto>() with { Id = id };

        try
        {
            var cmd = new UpdateProductCommand(dto);
            var res = await _sender.Send(cmd, ct);
            return Ok(res);
        }
        catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { message = "Sku or Slug already exists" });
        }
    }

    // DELETE /api/v1/products/{id}
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _sender.Send(new DeleteProductCommand(id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

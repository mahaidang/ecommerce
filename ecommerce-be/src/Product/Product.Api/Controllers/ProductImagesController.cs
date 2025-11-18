using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Features.Products.Commands;
using Product.Application.Features.Products.Queries;
using ProductService.Application.Products.Commands;

namespace Product.Api.Controllers;

[ApiController]
[Route("products/{productId}/images")]
//[ApiExplorerSettings(GroupName = "Product Images")]
public class ProductImagesController : ControllerBase
{
    private readonly ISender _sender;
    public ProductImagesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetImages(Guid productId, CancellationToken ct)
    {
        var cmd = new GetImagesByProdIdQuery(productId);
        var res = await _sender.Send(cmd, ct);
        return Ok(res);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Upload(Guid productId, IFormFile file, [FromQuery] bool isMain, CancellationToken ct)
    {
        var cmd = new UploadProductImageCommand(productId, file, isMain);
        var ok = await _sender.Send(cmd, ct);
        return ok ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete("{publicId}")]  
    public async Task<IActionResult> Delete(Guid productId, string publicId, CancellationToken ct)
    {
        var ok = await _sender.Send(new DeleteProductImageCommand(productId, publicId), ct);
        return ok ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpPost("{publicId}/main")]
    public async Task<IActionResult> SetMain(Guid productId, string publicId, CancellationToken ct)
    {
        var decoded = Uri.UnescapeDataString(publicId);
        var ok = await _sender.Send(new SetMainProductImageCommand(productId, decoded), ct);
        return ok ? Ok() : NotFound();
    }
}


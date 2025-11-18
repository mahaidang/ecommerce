using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Contracts.Category;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Categories.Commands.CreateCategory;

namespace Product.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _sender;
        public CategoriesController(ICategoryRepository repo, ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest req, CancellationToken ct)
        {
            var dto = req.Adapt<CategoryDto>();
            var cmd = new CreateCategoryCommand(dto);
            var res = await _sender.Send(cmd, ct);
            var response = res.Adapt<CategoryCreateResponse>();
            return CreatedAtAction(nameof(Create), new { id = response.Id }, res);
        }
    }
}

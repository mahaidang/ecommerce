using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReportService.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("revenue/{year:int}")]
    public async Task<IActionResult> GetRevenueByYear(int year, CancellationToken ct)
        => Ok(await _sender.Send(new GetRevenueByMonthQuery(year), ct));

    [HttpGet("products/top/{top:int}")]
    public async Task<IActionResult> GetTopSellingProducts(int top, CancellationToken ct)
        => Ok(await _sender.Send(new GetTopSellingProductsQuery(top), ct));

    [HttpGet("orders/excel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var file = await _sender.Send(new ExportToExcelQuery(from, to), ct);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Orders_{DateTime.UtcNow:yyyyMMddHHmm}.xlsx");
    }
}

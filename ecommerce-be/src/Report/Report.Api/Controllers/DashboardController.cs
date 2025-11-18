using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Application.Features.Queries;

namespace ReportService.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("/revenue-by-date")]
    public async Task<IActionResult> GetRevenueByDate([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await _sender.Send(new GetRevenueByDateQuery(from, to), ct);
        return Ok(result);
    }

    [HttpGet("/revenue-by-payment")]
    public async Task<IActionResult> GetRevenueByPaymentProvider([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await _sender.Send(new GetRevenueByPaymentProviderQuery(from, to), ct);
        return Ok(result);
    }
    [HttpGet("/order-status-count")]
    public async Task<IActionResult> GetOrderStatusCounts(CancellationToken ct)
    {
        var result = await _sender.Send(new GetOrderStatusCountsQuery(), ct);
        return Ok(result);
    }
    [HttpGet("/payment-summary")]
    public async Task<IActionResult> GetPaymentEventSummary(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPaymentEventSummaryQuery(), ct);
        return Ok(result);
    }
}

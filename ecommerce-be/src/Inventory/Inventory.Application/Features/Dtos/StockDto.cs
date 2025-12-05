namespace Inventory.Application.Features.Dtos;

public class StockDto
{
    public Guid ProductId { get; set; }
    public long AvailableQty { get; set; }
}

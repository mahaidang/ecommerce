using Microsoft.EntityFrameworkCore;
using Orchestrator.Worker.Models;

namespace Orchestrator.Worker.Repositories;

public class OrderSagaRepository
{
    private readonly SagaDbContext _db;

    public OrderSagaRepository(SagaDbContext db) => _db = db;

    public async Task<OrderSagaState?> GetByOrderIdAsync(Guid orderId)
    {
        return await _db.OrderSagaStates
                    .Include(x => x.OrderSagaItems)
                    .FirstOrDefaultAsync(x => x.OrderId == orderId);
    }
    public async Task SaveAsync(OrderSagaState state)
    {
        try
        {
            await _db.OrderSagaStates.AddAsync(state);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving saga: {ex.Message}");
            throw;
        }
    }


    public async Task UpdateAsync(OrderSagaState state)
    {
        _db.OrderSagaStates.Update(state);
        await _db.SaveChangesAsync();
    }
}

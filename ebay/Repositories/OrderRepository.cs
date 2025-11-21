using ebay.Data;
using ebay.Models;
namespace ebay.Repositories;
public interface IOrderRepository
{
    Task AddOrderAsync(Order order);
}

public class OrderRepository(EBayDbContext _context) : IOrderRepository
{

    public async Task AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }
}

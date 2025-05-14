using Microsoft.EntityFrameworkCore;
using NotifyNet.Core.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;

namespace NotifyNet.Infrastructure.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbConetxt _context;

    public OrderRepository(AppDbConetxt context)
    {
        _context = context;
    }
    
    public async Task<Order> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<Order> GetByNameAsync(string Name)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Name == Name);
    }

    public async Task AddAsync(Order pOrder)
    {
        await _context.Orders.AddAsync(pOrder);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid orderId)
    {
        _context.Orders.Remove(await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId));
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .ToListAsync();
    }
}
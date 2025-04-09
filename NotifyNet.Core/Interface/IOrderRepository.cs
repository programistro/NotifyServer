using AXO.Core.Models;

namespace NotifyNet.Core.Interface;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid orderId);
    
    Task<Order> GetByNameAsync(string name);
    
    Task AddAsync(Order pOrder);
    
    Task Update(Order order);
    
    Task Delete(Guid orderId);
    
    Task<IEnumerable<Order>> GetAllAsync();
}
using AXO.Core.Models;
using NotifyNet.Core.Dto;

namespace NotifyNet.Application.Interface;

public interface IOrderService
{
    Task<Order> GetByIdAsync(Guid orderId);
    
    Task<IEnumerable<Order>> GetAllAsync();
    
    Task<Order> GetByNameAsync(string name);
    
    Task AddAsync(Order order);
    
    Task UpdateAsync(Order order);
    
    Task DeleteAsync(Guid guid);
    
    Task<Order> AddDtoAsync(OrderDto order);
}
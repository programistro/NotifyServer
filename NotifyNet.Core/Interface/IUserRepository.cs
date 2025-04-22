using AXO.Core.Models;

namespace NotifyNet.Core.Interface;

public interface IUserRepository
{
    Task<Employee> GetByIdAsync(Guid userId);
    
    Task<Employee> GetByNameAsync(string name);
    
    Task<Employee> GetByEmailAsync(string email);
    
    Task AddAsync(Employee user);
    
    Task Update(Employee user);
    
    Task Delete(Guid userId);
    
    Task<IEnumerable<Employee>> GetAllAsync();
}
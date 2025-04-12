using NotifyNet.Core.Models;

namespace NotifyNet.Core.Interface;

public interface IUserRepository
{
    Task<User> GetByIdAsync(string userId);
    
    Task<User> GetByNameAsync(string name);
    
    Task<User> GetByEmailAsync(string email);
    
    Task AddAsync(User user);
    
    Task Update(User user);
    
    Task Delete(string userId);
    
    Task<IEnumerable<User>> GetAllAsync();
}
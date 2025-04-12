using NotifyNet.Core.Dto;
using NotifyNet.Core.Models;

namespace NotifyNet.Application.Interface;

public interface IUserService
{
    Task<User> GetByIdAsync(string userId);
    
    Task<User> GetByNameAsync(string name);
    
    Task<User> GetByEmailAsync(string email);
    
    Task<User> AddAsync(UserDto user);
    
    Task Update(User user);
    
    Task Delete(string userId);
    
    Task<IEnumerable<User>> GetAllAsync();
    
    Task<string> CreatePasswordHash(string password);
}
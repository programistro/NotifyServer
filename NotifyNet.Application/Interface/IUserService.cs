using AXO.Core.Models;
using NotifyNet.Core.Dto;

namespace NotifyNet.Application.Interface;

public interface IUserService
{
    Task<Employee> GetByIdAsync(Guid id);
    
    Task<Employee> GetByNameAsync(string name);
    
    Task<Employee> GetByEmailAsync(string email);
    
    Task<Employee> AddAsync(UserDto user);
    
    Task Update(Employee user);
    
    Task Delete(Guid id);
    
    Task<IEnumerable<Employee>> GetAllAsync();
    
    Task<string> CreatePasswordHash(string password);
}
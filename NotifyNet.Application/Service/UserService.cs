using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Interface;
using NotifyNet.Core.Models;

namespace NotifyNet.Application.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<Employee> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is not null)
        {
            return user;
        }
        
        return null;
    }

    public async Task<Employee> GetByNameAsync(string name)
    {
        var user = await _userRepository.GetByNameAsync(name);

        if (user is not null)
        {
            return user;
        }
        
        return null;
    }

    public async Task<Employee> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is not null)
        {
            return user;
        }
        
        return null;
    }

    public async Task<Employee> AddAsync(UserDto dto)
    {
        Employee user = new()
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Name,
            Name = dto.Name,
            PasswordHash = await CreatePasswordHash(dto.Password),
            ServiceNumber = Guid.NewGuid().ToString(),
            Photo = Guid.NewGuid().ToString(),
        };
        
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task Update(Employee user)
    {
        await _userRepository.Update(user);
    }

    public async Task Delete(Guid id)
    {
        await _userRepository.Delete(id);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<string> CreatePasswordHash(string password)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                builder.Append(hashValue[i].ToString("x2")); // Преобразуем байты хэша в шестнадцатеричное представление
            }

            return builder.ToString();
        }
    }
}
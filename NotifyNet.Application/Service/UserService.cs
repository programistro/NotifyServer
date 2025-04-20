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
    
    public async Task<User> GetByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is not null)
        {
            return user;
        }
        
        return null;
    }

    public async Task<User> GetByNameAsync(string name)
    {
        var user = await _userRepository.GetByNameAsync(name);

        if (user is not null)
        {
            return user;
        }
        
        return null;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is not null)
        {
            return user;
        }
        
        return null;
    }

    public async Task<User> AddAsync(UserDto dto)
    {
        User user = new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = dto.Email,
            UserName = dto.Name,
            Name = dto.Name,
            PasswordHash = await CreatePasswordHash(dto.Password),
        };
        
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task Update(User user)
    {
        await _userRepository.Update(user);
    }

    public async Task Delete(string userId)
    {
        await _userRepository.Delete(userId);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
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
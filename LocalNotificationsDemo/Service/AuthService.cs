using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AXO.Core.Models;
using NotifyNet.Application.Interface;

namespace LocalNotificationsDemo.Service;

// Services/IAuthService.cs
public interface IAuthService
{
    Task<bool> RegisterAsync(Employee user);
    Task<bool> LoginAsync(string username, string password);
    void Logout();
    bool IsAuthenticated { get; }
    string Username { get; }
}

// Services/AuthService.cs
public class AuthService(IUserService _userService) : IAuthService
{
    private readonly List<Employee> _users = new();
    private bool _isAuthenticated;
    private string _username;

    public bool IsAuthenticated => _isAuthenticated;
    public string Username => _username;

    public Task<bool> RegisterAsync(Employee user)
    {
        if (_users.Any(u => u.Name == user.Name || u.Email == user.Email))
        {
            return Task.FromResult(false);
        }

        _users.Add(user);
        return Task.FromResult(true);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var passwordHash = await _userService.CreatePasswordHash(password);
        
        var user = _users.FirstOrDefault(u => u.Name == username && u.PasswordHash == passwordHash);
        
        if (user != null)
        {
            _isAuthenticated = true;
            _username = user.Name;
            return true;
        }
        
        return false;
    }

    public void Logout()
    {
        _isAuthenticated = false;
        _username = null;
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AXO.Core.Models;
using LocalNotificationsDemo.Interfaces;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;

namespace LocalNotificationsDemo.Service;


public class AuthService(IUserService _userService, HttpClient _httpClient) : IAuthService
{
    public string JwtToken { get; private set; }

    public async Task<string> RegisterAsync(string email, string password, string username)
    {
        var user = new UserDto { Email = email, Password = password, Name = username };
        var json = JsonSerializer.Serialize(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.re.souso.ru/Auth/register", content);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        JwtToken = await response.Content.ReadAsStringAsync();
        return JwtToken;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = new UserDto { Email = email, Password = password };
        var json = JsonSerializer.Serialize(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.re.souso.ru/Auth/login", content);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        JwtToken = await response.Content.ReadAsStringAsync();
        return JwtToken;
    }

    public void SetAuthHeader()
    {
        if (!string.IsNullOrEmpty(JwtToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
    }
}
using System;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using NotifyNet.Core.Dto;

namespace NotifyNet.Application.Service;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<string> RegisterAsync(string email, string password)
    {
        var dto = new UserDto { Email = email, Password = password };

        var response = await _httpClient.PostAsJsonAsync("auth/register", dto, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            // throw new AuthException($"Registration failed: {errorContent}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var dto = new UserDto { Email = email, Password = password };

        var response = await _httpClient.PostAsJsonAsync("auth/login", dto, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            // throw new AuthException($"Login failed: {errorContent}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}

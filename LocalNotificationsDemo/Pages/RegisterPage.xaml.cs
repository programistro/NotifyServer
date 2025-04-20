using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNotificationsDemo.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using NotifyNet.Application.Interface;

namespace LocalNotificationsDemo.Pages;

public partial class RegisterPage : ContentPage
{
    public string Username { get; set; }
    public  string Password { get; set; }
    public  string Email { get; set; }
    public string ConfirmPassword { get; set; }
    
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    
    public RegisterPage(IAuthService authService, IUserService userService)
    {
        InitializeComponent();
        BindingContext = this;
        _authService = authService;
        _userService = userService;
    }

    private async void Register_OnClicked(object? sender, EventArgs e)
    {
        if (await _userService.CreatePasswordHash(Password) == await _userService.CreatePasswordHash(ConfirmPassword))
        {
            var token = await _authService.RegisterAsync(Email, Password, Username);

            if (token != null)
            {
                _authService.SetAuthHeader();
                await SecureStorage.Default.SetAsync("jwt_token", token);
                
                Routing.RegisterRoute("MainPage", typeof(MainPage));
                await Shell.Current.GoToAsync("MainPage");
            }
        }
    }
}
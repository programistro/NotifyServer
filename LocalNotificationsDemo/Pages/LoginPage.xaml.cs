using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNotificationsDemo.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using NotifyNet.Application.Interface;

namespace LocalNotificationsDemo.Pages;

public partial class LoginPage : ContentPage
{
    public string Email { get; set; }
    public string Password { get; set; }
    
    private readonly IAuthService _authService;

    public LoginPage(IAuthService authService)
    {
        InitializeComponent();

        BindingContext = this;
        
        _authService = authService;
    }

    private async void Login_OnClick(object? sender, EventArgs e)
    {
        var token = await _authService.LoginAsync(Email, Password);

        if (token != null)
        {
            _authService.SetAuthHeader();
            await SecureStorage.Default.SetAsync("jwt_token", token);
            
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Ошибка", "Не верный логин или пароль", "OK");
        }
    }
}
using System.Threading.Tasks;
using System.Windows.Input;
using AXO.Core.Models;
using LocalNotificationsDemo.Service;
using Microsoft.Maui.Controls;
using NotifyNet.Core.Models;

namespace LocalNotificationsDemo.ViewModels;

// ViewModels/RegisterViewModel.cs
public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        
        RegisterCommand = new Command(async () => await RegisterAsync());
        GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//Login"));
    }

    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || 
            string.IsNullOrWhiteSpace(Email) || 
            string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "All fields are required", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Error", "Passwords don't match", "OK");
            return;
        }

        var user = new Employee
        {
            Name = Username,
            Email = Email,
            PasswordHash = Password // В реальном приложении хэшируйте пароль!
        };

        var isRegistered = await _authService.RegisterAsync(user);
        
        if (isRegistered)
        {
            await Shell.Current.DisplayAlert("Success", "Registration successful", "OK");
            await Shell.Current.GoToAsync("//Login");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Username or email already exists", "OK");
        }
    }
}
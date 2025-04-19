using System.Windows.Input;
using LocalNotificationsDemo.Service;

namespace LocalNotificationsDemo.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    public string Username { get; set; }

    public string Password { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        
        LoginCommand = new Command(async () => await LoginAsync());
        GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//Register"));
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Username and password are required", "OK");
            return;
        }

        var isAuthenticated = await _authService.LoginAsync(Username, Password);
        
        if (isAuthenticated)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Invalid username or password", "OK");
        }
    }
}
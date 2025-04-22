using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Windows.Input;
using LocalNotificationsDemo.Interfaces;
using LocalNotificationsDemo.Pages;
using LocalNotificationsDemo.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
#if ANDROID
using LocalNotificationsDemo.Platforms.Android;
#endif

namespace LocalNotificationsDemo;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    INotificationManagerService notificationManager;
    int notificationNumber = 0;
    private readonly IAuthService _authService;
    
    private bool isAuthenticated;
    public bool IsAuthenticated
    {
        get => isAuthenticated;
        set
        {
            isAuthenticated = value;
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }
    
    public ICommand LogoutCommand { get; }

    public MainPage()
    {
        InitializeComponent();
        
        BindingContext = this;
        
        notificationManager = IPlatformApplication.Current.Services.GetRequiredService<INotificationManagerService>();
        _authService = IPlatformApplication.Current.Services.GetRequiredService<IAuthService>();
        notificationManager.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;
            ShowNotification(eventData.Title, eventData.Message);
        };
    }

#if ANDROID
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PermissionStatus status = await Permissions.RequestAsync<NotificationPermission>();
    }
#endif

    void OnSendClick(object sender, EventArgs e)
    {
        notificationNumber++;
        string title = $"Local Notification #{notificationNumber}";
        string message = $"You have now received {notificationNumber} notifications!";
        notificationManager.SendNotification(title, message);
    }

    void OnScheduleClick(object sender, EventArgs e)
    {
        notificationNumber++;
        string title = $"Local Notification #{notificationNumber}";
        string message = $"You have now received {notificationNumber} notifications!";
        notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
    }

    void ShowNotification(string title, string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var msg = new Label()
            {
                Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
            };
            verticalStackLayout.Children.Add(msg);
        });
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        await Shell.Current.GoToAsync("RegisterPage");
    }

    private async void MainPage_OnLoaded(object? sender, EventArgs e)
    { 
        var token = await SecureStorage.Default.GetAsync("jwt_token");

        if (token != null)
        {
            IsAuthenticated = false;
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var email = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            var name = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.GivenName).Value;
        }
        else
        {
            IsAuthenticated = true;
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

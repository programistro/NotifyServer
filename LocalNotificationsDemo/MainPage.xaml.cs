using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Windows.Input;
using AXO.Core.Models;
using LocalNotificationsDemo.Interfaces;
using LocalNotificationsDemo.Pages;
using LocalNotificationsDemo.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using NotifyNet.Application.Interface;
#if ANDROID
using LocalNotificationsDemo.Platforms.Android;
#endif

namespace LocalNotificationsDemo;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    INotificationManagerService notificationManager;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private ObservableCollection<Order> _orders;

    public ObservableCollection<Order> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
        }
    }
    
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

    public MainPage()
    {
        InitializeComponent();
        
        BindingContext = this;
        
        notificationManager = IPlatformApplication.Current.Services.GetRequiredService<INotificationManagerService>();
        _authService = IPlatformApplication.Current.Services.GetRequiredService<IAuthService>();
        _userService = IPlatformApplication.Current.Services.GetRequiredService<IUserService>();
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
    
    private void OnOrderCreated(Order obj)
    {
        Orders.Add(obj);
    }
#endif

    private async void MainPage_OnLoaded(object? sender, EventArgs e)
    { 
        var token = await SecureStorage.Default.GetAsync("jwt_token");

        if (token != null)
        {
            IsAuthenticated = false;
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var email = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

            var user = await _userService.GetByEmailAsync(email);
            
            user.OrdersChanged += UserOnOrdersChanged;
            Orders = new ObservableCollection<Order>(user.Orders);
            
            OnPropertyChanged(nameof(Orders));
        }
        else
        {
            IsAuthenticated = true;
        }
    }

    private void UserOnOrdersChanged(ObservableCollection<Order> sender)
    {
        Orders = sender;
        OnPropertyChanged(nameof(Orders));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}

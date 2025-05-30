using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Windows.Input;
using LocalNotificationsDemo.Interfaces;
using LocalNotificationsDemo.Pages;
using LocalNotificationsDemo.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;
using Plugin.FirebasePushNotifications;
#if ANDROID
using LocalNotificationsDemo.Platforms.Android;
#endif

namespace LocalNotificationsDemo;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private readonly ILogger<MainPage> _logger;
    INotificationManagerService notificationManager;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;
    private readonly HttpClient _httpClient;
    private string _token = string.Empty;
    
    private string _email = string.Empty;

    public string Email
    {
        get { return _email; }
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }
    

    private Employee _user;
    private ObservableCollection<Order> _orders;

    public ObservableCollection<Order> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            OnPropertyChanged(nameof(Orders));
        }
    }

    private Employee _employeeApplicant;
    
    public Employee EmployeeApplicatint
    {
        get
        {
            return _employeeApplicant;
        }
        set
        {
            _employeeApplicant = value;
            OnPropertyChanged(nameof(EmployeeApplicatint));
        }
    }

    private bool _isAuthenticated;
    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set
        {
            _isAuthenticated = value;
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }
    
    public ICommand OpenUrlCommand => new Command<String>(OpenUrl);

    private async void OpenUrl(string obj)
    {
        await Browser.OpenAsync(obj);
    }

    public ICommand AuthCommand => new Command(Login);

    private async void Login()
    {
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        await Shell.Current.GoToAsync("LoginPage");
    }

    public MainPage()
    {
        InitializeComponent();
        
        BindingContext = this;
        IsAuthenticated = false;
        
        _httpClient = IPlatformApplication.Current.Services.GetService<HttpClient>();
        notificationManager = IPlatformApplication.Current.Services.GetRequiredService<INotificationManagerService>();
        _logger = IPlatformApplication.Current.Services.GetService<ILogger<MainPage>>();
        _userService = IPlatformApplication.Current.Services.GetRequiredService<IUserService>();
        _orderService = IPlatformApplication.Current.Services.GetRequiredService<IOrderService>();
        // notificationManager.NotificationReceived += (sender, eventArgs) =>
        // {
        //     var eventData = (NotificationEventArgs)eventArgs;
        //     ShowNotification(eventData.Title, eventData.Message);
        // };
        notificationManager.NotificationReceived += NotificationManagerOnNotificationReceived;
    }

    private void NotificationManagerOnNotificationReceived(object? sender, EventArgs e)
    {
        var eventData = (NotificationEventArgs)e;
        ShowNotification(eventData.Title, eventData.Message);
    }

#if ANDROID
    protected override async void OnAppearing()
    {
        PermissionStatus status = await Permissions.RequestAsync<NotificationPermission>();

        if (string.IsNullOrEmpty(_token))
        {
            _token = await SecureStorage.Default.GetAsync("jwt_token");

            if (_token == null)
            {
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(_token);

            Email = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var respone = await _httpClient.GetAsync($"https://api.re.souso.ru/User/get-user-by-email?email={Email}");
            var json = await respone.Content.ReadAsStringAsync();
            _user = await respone.Content.ReadFromJsonAsync<Employee>();

            _user.OrdersChanged += UserOnOrdersChanged;
            Orders = new ObservableCollection<Order>(_user.Orders);

            OnPropertyChanged(nameof(Orders));

            IsAuthenticated = true;
        }
    }
#endif

    private void OnOrderCreated(Order obj)
    {
        Orders.Add(obj);
    }

    private async void MainPage_OnLoaded(object? sender, EventArgs e)
    {
        INotificationPermissions notificationPermissions = INotificationPermissions.Current;
        var permissionStatus = await notificationPermissions.RequestPermissionAsync();

        IFirebasePushNotification.Current.TokenRefreshed += this.OnTokenRefresh;
        IFirebasePushNotification.Current.NotificationOpened += this.OnNotificationOpened;
        IFirebasePushNotification.Current.NotificationReceived += this.OnNotificationReceived;
        IFirebasePushNotification.Current.NotificationDeleted += this.OnNotificationDeleted;
        IFirebasePushNotification.Current.NotificationAction += this.OnNotificationAction;
        IFirebasePushNotification.Current.SubscribeTopics(["order_updated, order_created"]);
        IFirebasePushNotification.Current.RegisterForPushNotificationsAsync();
        
        _logger.LogInformation("Registering notification categories");
    }

    public void UserOnOrdersChanged(ObservableCollection<Order> sender)
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
            //var msg = new Label()
            //{
            //    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
            //};
            //verticalStackLayout.Children.Add(msg);
        });
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        await Shell.Current.GoToAsync("LoginPage");
    }
    
    private void OnNotificationAction(object? sender, FirebasePushNotificationActionEventArgs e)
    {
        _logger.LogInformation($"NotificationAction: {e.Action}");

        OpenUrl("https://re.souso.ru/");
    }

    private void OnNotificationDeleted(object? sender, FirebasePushNotificationDataEventArgs e)
    {
        _logger.LogInformation($"NotificationDeleted: {e.Data}");
    }

    private async void OnNotificationReceived(object? sender, FirebasePushNotificationDataEventArgs e)
    {
        _logger.LogInformation($"NotificationReceived: {e.Data}");

        var order = JsonConvert.DeserializeObject<Order>(e.Data["order"].ToString());
        
        if (_user != null)
        {
            _user?.Orders.Add(order);
            Orders.Add(order);
            try
            {
                await _userService.Update(_user);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        
        notificationManager.SendNotification("Уведомление", $"Создана новая запись {order.Name}");
    }

    private void OnNotificationOpened(object? sender, FirebasePushNotificationResponseEventArgs e)
    {
        _logger.LogInformation($"NotificationOpened: {e.Data}");

        OpenUrl("https://re.souso.ru/lk");
    }

    private void OnTokenRefresh(object? sender, FirebasePushNotificationTokenEventArgs e)
    {
        _logger.LogInformation($"TokenRefresh: {e.Token}");
    }

    private async void DeleteOrderClick(object? sender, EventArgs e)
    {
        var menuitem = sender as MenuItem;
        
        if (menuitem != null)
        {
            var order = menuitem.BindingContext as Order;
            
            var findOrder = await _orderService.GetByIdAsync(order.Id);

            if (findOrder != null)
            {
                var itemsToRemove = _user.Orders.Where(o => o.Id == order.Id).ToList();
                foreach (var item in itemsToRemove)
                {
                    _user.Orders.Remove(item);
                }

                // _user.Orders.Remove(findOrder);
                _userService.Update(_user);
                Orders.Remove(order);
            }
        }
    }

    private void OrderList_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        OpenUrl("https://re.souso.ru/worker");
    }
}

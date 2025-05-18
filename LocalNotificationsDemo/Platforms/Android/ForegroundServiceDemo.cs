using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using NotifyNet.Core.Models;
using Java.Lang;
using Java.Util.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using NotifyNet.Core.Dto;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Interface;
using NotifyNet.Infrastructure.Data;

namespace LocalNotificationsDemo.Platforms.Android;

[Service]
public class ForegroundServiceDemo : global::Android.App.Service
{
    private readonly INotificationManagerService _notificationManagerService;
    private static bool IsStarted { get; set; }
    private HubConnection _hubConnection;
    private readonly ILogger<ForegroundServiceDemo> _logger;
    private readonly IUserService _userService;
    private readonly MainPage _mainPage;

    private string _token { get; set; }
    private Employee _employee { get; set; }

    private string _email { get; set; }

    public ForegroundServiceDemo()
    {
        _notificationManagerService = MauiApplication.Current.Services.GetService<INotificationManagerService>();
        _userService = MauiApplication.Current.Services.GetService<IUserService>();
        _logger = MauiApplication.Current.Services.GetService<ILogger<ForegroundServiceDemo>>();
        _mainPage = MauiApplication.Current.Services.GetService<MainPage>();
        _notificationManagerService.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;
            ShowNotification(eventData.Title, eventData.Message);
        };
    }

    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    void ShowNotification(string title, string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var msg = new Label()
            {
                Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
            };
        });
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        Task.Run(async () =>
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://api.re.souso.ru/orderHub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<Order>("OrderCreated", OrderCreated);
            _hubConnection.On<Order>("OrderUpdated", OrderUpdated);
            _hubConnection.On<Guid>("OrderDeleted", OrderDeleted);
            
            _token = await SecureStorage.Default.GetAsync("jwt_token");

            if (_token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(_token);

                _email = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                _employee = await _userService.GetByEmailAsync(_email);

                _employee.OrdersChanged += _mainPage.UserOnOrdersChanged;
            }

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            _hubConnection.Reconnected += async (error) =>
            {
                _logger.LogInformation("SignalR hub connection started again");
                _notificationManagerService.SendNotification("Уведомление", "Соединение снова установленно");
            };

            try
            {
                await _hubConnection.StartAsync();
                _logger.LogInformation("SignalR hub connection started");
                _notificationManagerService.SendNotification("Уведомление", "Соединение установленно");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new System.Exception(ex.Message);
            }

            // _notificationManagerService.SendNotification("adwad", "dawdwad");

            // await _hubConnection.InvokeAsync("Send", "connect");
            
            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (IsStarted)
                {
                    Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
                    _notificationManagerService.SendNotification("adwad", "dawdwad");
                }
                else
                {
                    Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
                    IsStarted = true;
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
                StopForeground(StopForegroundFlags.Remove);
                StopSelf();
            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
            {
                Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
            }
        });
        
        return StartCommandResult.Sticky;
    }

    private async void OrderCreated(Order order)
    {
        try
        {
            if (_employee == null)
            {
                _token = await SecureStorage.Default.GetAsync("jwt_token");

                if (_token != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(_token);

                    _email = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                    _employee = await _userService.GetByEmailAsync(_email);
                
                    _employee.OrdersChanged += _mainPage.UserOnOrdersChanged;
                }
            }
            else
            {
                if (order.Created.HasValue)
                    order.Created = order.Created.Value.ToUniversalTime();

                _employee?.Orders.Add(order);
                try
                {
                    await _userService.Update(_employee);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                _logger.LogInformation("Order created");

                _notificationManagerService.SendNotification("Уведомление", $"ордер создан {order.Name}", link: "https://re.souso.ru/notifications");
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e.Message);
            throw;
        }
    }

    private void OrderUpdated(Order order)
    {
        _notificationManagerService.SendNotification("dwdwadwa", "wadwadw");
    }

    private void OrderDeleted(Guid orderId)
    {
        _notificationManagerService.SendNotification("dwdwadwa", "wadwadw");
    }
}
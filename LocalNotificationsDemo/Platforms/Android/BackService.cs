using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using AXO.Core.Models;
using Java.Lang;
using Java.Util.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using NotifyNet.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalNotificationsDemo.Platforms.Android;

[Service]
public class ForegroundServiceDemo : global::Android.App.Service
{
    private readonly INotificationManagerService _notificationManagerService;
    private static bool IsStarted { get; set; }
    private HubConnection _hubConnection;
    private readonly ILogger<ForegroundServiceDemo> _logger;

    public ForegroundServiceDemo()
    {
        _notificationManagerService = MauiApplication.Current.Services.GetService<INotificationManagerService>();
        _logger = MauiApplication.Current.Services.GetService<ILogger<ForegroundServiceDemo>>();
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
        _hubConnection = new HubConnectionBuilder()
           .WithUrl("https://192.168.1.83:8080/orderHub") // Замените на реальный URL вашего API
           .WithAutomaticReconnect() // Автоматическое переподключение при разрыве
           .Build();

        _hubConnection.On<OrderDto>("OrderCreated", OrderCreated);
        _hubConnection.On<Order>("OrderUpdated", OrderUpdated);
        _hubConnection.On<Guid>("OrderDeleted", OrderDeleted);

        _hubConnection.InvokeAsync("Send", "connect");

        // Обработка ошибок подключения
        _hubConnection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _hubConnection.StartAsync();
        };

        try
        {
            _hubConnection.StartAsync();
            _logger.LogInformation("SignalR hub connection started");
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw new System.Exception(ex.Message);
        }

        //_notificationManagerService.SendNotification("adwad", "dawdwad");

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

        return StartCommandResult.Sticky;
    }

    private void OrderCreated(OrderDto order)
    {
        _notificationManagerService.SendNotification("dwdwadwa", "wadwadw");
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
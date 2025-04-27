using System;
using System.Threading;
using System.Threading.Tasks;
using AXO.Core.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using NotifyNet.Core.Dto;

namespace LocalNotificationsDemo.Service;

public class OrderBackgroundService : BackgroundService
{
    private readonly ILogger<OrderBackgroundService> _logger;
    private readonly INotificationManagerService _notificationManagerService;
    private HubConnection _hubConnection;

    public OrderBackgroundService(ILogger<OrderBackgroundService> logger, INotificationManagerService notificationManagerService)
    {
        _logger = logger;
        _notificationManagerService = notificationManagerService;
        _notificationManagerService.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;
            ShowNotification(eventData.Title, eventData.Message);
        };
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Инициализация подключения к SignalR хабу
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://api.re.souso.ru/orderHub") // Замените на реальный URL вашего API
            .WithAutomaticReconnect() // Автоматическое переподключение при разрыве
            .Build();

        // Регистрация обработчиков событий
        _hubConnection.On<OrderDto>("OrderCreated", OrderCreated);
        _hubConnection.On<Order>("OrderUpdated", OrderUpdated);
        _hubConnection.On<Guid>("OrderDeleted", OrderDeleted);

        OrderCreated(new OrderDto());

        // Обработка ошибок подключения
        _hubConnection.Closed += async (error) =>
        {
            _logger.LogError(error, "Connection to SignalR Hub was closed.");
            await Task.Delay(new Random().Next(0, 5) * 1000, stoppingToken);
            await _hubConnection.StartAsync(stoppingToken);
        };

        try
        {
            await _hubConnection.StartAsync(stoppingToken);
            _logger.LogInformation("SignalR Hub connection started.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting SignalR Hub connection.");
        }

        // Ожидание отмены
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        // Закрытие подключения при остановке сервиса
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    private void OrderCreated(OrderDto order)
    {
        _logger.LogInformation($"Order created: {order.Name}");
        // Здесь можно добавить логику обработки создания заказа
        _notificationManagerService.SendNotification("dwdwadwa", "wadwadw");
    }

    private void OrderUpdated(Order order)
    {
        _logger.LogInformation($"Order updated: {order.Id}");
        // Здесь можно добавить логику обработки обновления заказа
    }

    private void OrderDeleted(Guid orderId)
    {
        _logger.LogInformation($"Order deleted: {orderId}");
        // Здесь можно добавить логику обработки удаления заказа
    }
}
using Microsoft.AspNetCore.SignalR;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Web.Hubs;

namespace NotifyNet.Web.Service;

public class CheckerBackgroundService : BackgroundService
{
    private List<Order> Orders { get; set; } = new();
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _orderHub;
    private readonly ILogger<CheckerBackgroundService> _logger;
    private readonly ConnectionManager _connectionManager;

    public CheckerBackgroundService(IOrderService orderService, ILogger<CheckerBackgroundService> logger,
        ConnectionManager connectionManager)
    {
        _orderService = orderService;
        _logger = logger;
        _connectionManager = connectionManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (Orders.Count == 0)
            {
                var list = await _orderService.GetAllAsync();

                foreach (var item in list)
                {
                    Orders.Add(item);
                }
            }

            var newList = await _orderService.GetAllAsync();

            var onlyInFirst = Orders.Except(newList).ToList();

            if (onlyInFirst.Count > 0)
            {
                foreach (var item in onlyInFirst)
                {
                    Orders.Add(item);

                    await _orderHub.Clients.All.SendAsync("OrderCreated", item);
                }

                foreach (var item in _connectionManager.Users)
                {
                    _logger.LogInformation($"user: {item}");
                }
                foreach (var item in _connectionManager.Users)
                {
                    _logger.LogInformation($"user connect id: {item}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await Task.Delay(30000, stoppingToken);
    }
}
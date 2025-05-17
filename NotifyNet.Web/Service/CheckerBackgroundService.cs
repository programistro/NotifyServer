using Microsoft.AspNetCore.SignalR;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Web.Hubs;

namespace NotifyNet.Web.Service;

public class CheckerBackgroundService : BackgroundService
{
    private List<Order> Orders { get; set; } = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHubContext<OrderHub> _orderHub;
    private readonly ILogger<CheckerBackgroundService> _logger;
    private readonly ConnectionManager _connectionManager;

    public CheckerBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<CheckerBackgroundService> logger,
        ConnectionManager connectionManager)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _connectionManager = connectionManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                
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
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        
            foreach (var item in _connectionManager.Users)
            {
                _logger.LogInformation($"user: {item}");
            }
            foreach (var item in _connectionManager.ConnectionsId)
            {
                _logger.LogInformation($"user connect id: {item}");
            }

            await Task.Delay(30000, stoppingToken);
        }
    }
}
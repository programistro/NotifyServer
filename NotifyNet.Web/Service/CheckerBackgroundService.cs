using System.Text;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Web.Hubs;
using NotifyNet.Web.Models;

namespace NotifyNet.Web.Service;

public class CheckerBackgroundService : BackgroundService
{
    private List<Order> Orders { get; set; } = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHubContext<OrderHub> _orderHub;
    private readonly ILogger<CheckerBackgroundService> _logger;
    private readonly ConnectionManager _connectionManager;
    private readonly HttpClient _httpClient;

    public CheckerBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<CheckerBackgroundService> logger,
        ConnectionManager connectionManager, IHubContext<OrderHub> orderHub)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _connectionManager = connectionManager;
        _orderHub = orderHub;
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
                    newList.ToList();

                    var onlyInFirst = newList.Except(Orders).ToList();
                    var onlyInSecond = Orders.Except(newList).ToList();

                    if (onlyInFirst.Count < newList.Count())
                    {
                        foreach (var item in onlyInFirst)
                        {
                            Orders.Add(item);

                            await _orderHub.Clients.All.SendAsync("OrderCreated", item);
                        }
                    }
                    if(onlyInSecond.Count < newList.Count())
                    {
                        foreach (var item in onlyInSecond)
                        {
                            Orders.Remove(item);
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
            _logger.LogInformation($"orders count {Orders.Count}");

            await Task.Delay(10000, stoppingToken);
        }
    }
}
using AXO.Core.Models;
using Microsoft.AspNetCore.SignalR;
using NotifyNet.Application.Interface;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Web.Hubs;

namespace NotifyNet.Web.Service;

public class CheckerBackgroundService : BackgroundService
{
    private List<Order> Orders { get; set; } = new();
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _orderHub;

    public CheckerBackgroundService(IOrderService orderService)
    {
        _orderService = orderService;
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
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        await Task.Delay(30000, stoppingToken);
    }
}
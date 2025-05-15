using Microsoft.AspNetCore.SignalR;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Web.Hubs;

namespace NotifyNet.Web.Service;

public sealed class CheckerBackgroundService : BackgroundService
{
    private List<Order> Orders { get; set; } = new();
    // private readonly IOrderService _orderService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<OrderHub> _orderHub;

    public CheckerBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<OrderHub> orderHub)
    {
        // _orderService = orderService;
        _orderHub = orderHub;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    if (Orders.Count == 0)
                    {
                        var list = await _orderService.GetAllAsync();

                        Orders = list.ToList();
                    }

                    var newList = await _orderService.GetAllAsync();
                    newList.ToList();

                    var onlyInFirst = newList.Except(Orders).ToList();

                    if (onlyInFirst.Count < newList.Count())
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

            await Task.Delay(15000, stoppingToken);
        }
    }
}
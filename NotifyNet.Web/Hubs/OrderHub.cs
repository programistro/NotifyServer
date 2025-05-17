using AXO.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotifyNet.Core.Dto;
using NotifyNet.Web.Service;

namespace NotifyNet.Web.Hubs;

public class OrderHub(ILogger<OrderHub> _logger, ConnectionManager _manager) : Hub
{
    public async Task Send(string message)
    {
        _logger.LogInformation(message);
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"OnConnectedAsync: {Context.User.Identity.Name}");
        _manager.Users.Add(Context.User.Identity.Name);
        _manager.ConnectionsId.Add(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public async Task NotifyOrderCreated(OrderDto order)
    {
        await Clients.All.SendAsync("OrderCreated", order);
    }

    // Отправляет уведомление об обновлении заказа
    public async Task NotifyOrderUpdated(Order order)
    {
        await Clients.All.SendAsync("OrderUpdated", order);
    }

    // Отправляет уведомление об удалении заказа
    public async Task NotifyOrderDeleted(Guid orderId)
    {
        await Clients.All.SendAsync("OrderDeleted", orderId);
    }
}
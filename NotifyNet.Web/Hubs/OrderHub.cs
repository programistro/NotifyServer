using Microsoft.AspNetCore.SignalR;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Models;

namespace NotifyNet.Web.Hubs;

public class OrderHub(ILogger<OrderHub> _logger) : Hub
{
    public async Task Send(string message)
    {
        _logger.LogInformation(message);
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
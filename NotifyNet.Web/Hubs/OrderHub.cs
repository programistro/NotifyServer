using AXO.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotifyNet.Core.Dto;

namespace NotifyNet.Web.Hubs;

[Authorize]
public class OrderHub : Hub
{
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
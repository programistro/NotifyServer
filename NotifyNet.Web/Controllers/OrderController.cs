using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Models;
using NotifyNet.Web.Hubs;

namespace NotifyNet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService, IHubContext<OrderHub> hubContext)
    {
        _logger = logger;
        _orderService = orderService;
        _hubContext = hubContext;
    }

    [HttpGet("get-order-by-id")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        return Ok(await _orderService.GetByIdAsync(orderId));
    }

    [HttpGet("get-order-by-name")]
    public async Task<IActionResult> GetOrder(string name)
    {
        return Ok(await _orderService.GetByNameAsync(name));
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder(OrderDto dto)
    {
        var order = await _orderService.AddDtoAsync(dto);
        
        await _hubContext.Clients.All.SendAsync("OrderCreated", order);
        
        return Ok(order);
    }

    [HttpPut("update-order")]
    public async Task<IActionResult> UpdateOrder(Order order)
    {
        await _orderService.UpdateAsync(order);
        
        await _hubContext.Clients.All.SendAsync("OrderUpdated", order);
        
        return Ok(order);
    }

    [HttpDelete("delete-order")]
    public async Task<IActionResult> DeleteOrder(Guid orderId)
    {
        await _orderService.DeleteAsync(orderId);
        
        await _hubContext.Clients.All.SendAsync("OrderDeleted", orderId);
        
        return Ok();
    }
}
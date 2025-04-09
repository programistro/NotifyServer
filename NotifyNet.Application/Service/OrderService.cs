using AXO.Core.Models;
using Microsoft.Extensions.Logging;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Interface;
using NotifyNet.Infrastructure.Repository;

namespace NotifyNet.Application.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<IOrderService> _logger;

    public OrderService(IOrderRepository orderRepository, ILogger<IOrderService> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }
    
    public async Task<Order> GetByIdAsync(Guid orderId)
    {
        var findOrder = await _orderRepository.GetByIdAsync(orderId);

        if (findOrder == null)
        {
            return null;
        }
        
        return findOrder;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order> GetByNameAsync(string name)
    {
        var findOrder = await _orderRepository.GetByNameAsync(name);

        if (findOrder == null)
        {
            return null;
        }
        
        return findOrder;
    }

    public async Task AddAsync(Order order)
    {
        await _orderRepository.AddAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        await _orderRepository.Update(order);
    }

    public async Task DeleteAsync(Guid guid)
    {
        var findOrder = await _orderRepository.GetByIdAsync(guid);

        if (findOrder != null)
        {
            await _orderRepository.Delete(guid);
        }
    }

    public async Task<Order> AddDtoAsync(OrderDto dto)
    {
        Order order = new()
        {
            Id = Guid.NewGuid(),
            EmployeeApplicantId = dto.EmployeeApplicantId,
            Created = dto.Created,
            // OrderChats = dto.OrderChats,
            Description = dto.Description,
            Name = dto.Name,
            // Employees = dto.Employees,
            // Process = dto.Process,
            Updated = dto.Updated,
            BuildingId = dto.BuildingId,
            DateModeration = dto.DateModeration,
            DivisionId = dto.DivisionId,
            DescriptionDispathcer = dto.DescriptionDispathcer,
            // EmployeeApplicant = dto.EmployeeApplicant,
            EquipmentId = dto.EquipmentId,
            // OrderEmployees = dto.OrderEmployees,
            // EmployeeDispatcher = dto.EmployeeDispatcher,
            // EmployeeExecuter = dto.EmployeeExecuter,
            // EmployeeExecuters = dto.EmployeeExecuters,
            // EmployeeNotification = dto.EmployeeNotification,
            EventId = dto.EventId,
            OrderNumber = dto.OrderNumber,
            PriorityId = dto.PriorityId,
            RecordId = dto.RecordId,
            ProcessId = dto.ProcessId,
            DateOfExecution = dto.DateOfExecution,
            SupportId = dto.SupportId,
            DateWorkStatus = dto.DateWorkStatus,
            DescriptionOfWork = dto.DescriptionOfWork,
            DateOfClose = dto.DateOfClose,
            EmployeeDispatcherId = dto.EmployeeDispatcherId,
            EmployeeExecuterId = dto.EmployeeExecuterId,
            // EmployeeNotificationId = dto.EmployeeNotificationId,
        };
        
        await _orderRepository.AddAsync(order);

        return order;
    }
}
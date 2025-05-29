using Microsoft.Extensions.Logging;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Interface;
using NotifyNet.Core.Models;

namespace NotifyNet.Application.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<IOrderService> _logger;
    private readonly TimeZoneInfo _timeZoneInfo;

    public OrderService(IOrderRepository orderRepository, ILogger<IOrderService> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
    }
    
    public async Task<Order> GetByIdAsync(Guid orderId)
    {
        var findOrder = await _orderRepository.GetByIdAsync(orderId);

        if (findOrder == null)
        {
            return null;
        }
        
        findOrder.SetMoscowDate();
        
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
        
        findOrder.SetMoscowDate();
        
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
            // SupportId = dto.SupportId,
            DateWorkStatus = dto.DateWorkStatus,
            DescriptionOfWork = dto.DescriptionOfWork,
            DateOfClose = dto.DateOfClose,
            EmployeeDispatcherId = dto.EmployeeDispatcherId,
            EmployeeExecuterId = dto.EmployeeExecuterId,
            // EmployeeNotificationId = dto.EmployeeNotificationId,
        };
        
        order.SetMoscowDate();
        
        await _orderRepository.AddAsync(order);

        return order;
    }
}

public static class OrderServiceExtensions
{
    public static void SetMoscowDate<T>(this T entity)
    {
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        
        if (entity is null)
            return;
        
        var properties = entity.GetType().GetProperties()
            .Where(x => x.PropertyType == typeof(DateTime?) || x.PropertyType == typeof(DateTime));

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            
            if(value == null)
                continue;

            if (property.PropertyType == typeof(DateTime?))
            {
                var date = (DateTime?)value;
                if (date.HasValue)
                {
                    var mscDate = TimeZoneInfo.ConvertTimeFromUtc(date.Value, timeZoneInfo);
                    property.SetValue(entity, mscDate);
                }
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                var date = (DateTime)value;
                var mscDate = TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneInfo);
                property.SetValue(entity, mscDate);
            }
        }
    }
}
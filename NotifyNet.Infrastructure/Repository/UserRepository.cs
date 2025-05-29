using Microsoft.EntityFrameworkCore;
using NotifyNet.Core.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;

namespace NotifyNet.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbConetxt _context;

    public UserRepository(AppDbConetxt context)
    {
        _context = context;
    }
    
    public async Task<Employee> GetByIdAsync(Guid employeeId)
    {
        return await _context._Employees
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == employeeId);
    }

    public async Task<Employee> GetByNameAsync(string name)
    {
        return await _context._Employees
                .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<Employee> GetByEmailAsync(string email)
    {
        return await _context._Employees
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(Employee user)
    {
        await _context._Employees.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Employee user)
    {
        if (user.Orders != null)
        {
            user.Orders.NormalizeDatesToUtc();
        }
        
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid userId)
    {
        var user = await _context._Employees.FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            _context._Employees.Remove(user);
        
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context._Employees.ToListAsync();
    }
}

public static class DateTimeExtensions
{
    public static void NormalizeAllDatesToUtc<T>(this IEnumerable<T> entities)
    {
        if (entities == null) return;

        foreach (var entity in entities)
        {
            NormalizeDatesToUtc(entity);
        }
    }
    
    public static void NormalizeDatesToUtc<T>(this T entity)
    {
        if (entity == null) return;

        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(DateTime?) || p.PropertyType == typeof(DateTime));

        foreach (var prop in properties)
        {
            var value = prop.GetValue(entity);

            if (value == null)
                continue;

            if (prop.PropertyType == typeof(DateTime?))
            {
                var dt = (DateTime?)value;
                if (dt.HasValue)
                {
                    var utcDate = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
                    prop.SetValue(entity, utcDate);
                }
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                var dt = (DateTime)value;
                var utcDate = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                prop.SetValue(entity, utcDate);
            }
        }
    }
}

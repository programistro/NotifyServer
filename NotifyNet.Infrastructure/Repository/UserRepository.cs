using AXO.Core.Models;
using Microsoft.EntityFrameworkCore;
using NotifyNet.Core.Interface;
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
        return await _context.Employees
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == employeeId);
    }

    public async Task<Employee> GetByNameAsync(string name)
    {
        return await _context.Employees
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<Employee> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(Employee user)
    {
        await _context.Employees.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Employee user)
    {
        _context.Employees.Attach(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid userId)
    {
        var user = await _context.Employees.FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            _context.Employees.Remove(user);
        
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees.ToListAsync();
    }
}
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
    
    public async Task<User> GetByIdAsync(string orderId)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<User> GetByNameAsync(string name)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Update(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            _context.Users.Remove(user);
        
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
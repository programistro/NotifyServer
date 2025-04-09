using AXO.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace NotifyNet.Infrastructure.Data;

public class AppDbConetxt : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("host=localhost;port=5432;Username=postgres;Password=post;Database=postgres");
    }
}
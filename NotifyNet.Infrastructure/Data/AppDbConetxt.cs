using AXO.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NotifyNet.Infrastructure.Data;

public class AppDbConetxt : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql("host=localhost;port=5432;Username=postgres;Password=post;Database=postgres");
        optionsBuilder.UseNpgsql("host=83.222.17.62;port=5432;Username=postgres;Password=LapinBoss2022!;Database=RE");
    }
}
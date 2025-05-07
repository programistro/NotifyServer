using AXO.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NotifyNet.Infrastructure.Data;

public class AppDbConetxt : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    
    public DbSet<Employee> _Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Orders)
            .WithOne()
            .HasForeignKey(o => o.EmployeeApplicantId)
            .HasForeignKey(o => o.BuildingId)
            .HasForeignKey(o => o.ProcessId)
            .HasForeignKey(o => o.RecordId)
            .HasForeignKey(o => o.DivisionId)
            .HasForeignKey(o => o.EmployeeNotificationId)
            .HasForeignKey(o => o.EmployeeDispatcherId)
            .HasForeignKey(o => o.EmployeeExecuterId)
            .HasForeignKey(o => o.EquipmentId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql("host=localhost;port=5432;Username=postgres;Password=post;Database=postgres");
        optionsBuilder.UseNpgsql("host=83.222.17.62;port=5432;Username=postgres;Password=LapinBoss2022!;Database=RE");
    }
}
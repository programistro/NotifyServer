using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotifyNet.Core.Models;

namespace NotifyNet.Infrastructure.Data;

public class AppDbConetxt : IdentityDbContext<
        Employee, Permission, Guid,
        IdentityUserClaim<Guid>, EmployeePermission, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Employee> _Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>().ToTable("_Employees");
            
        builder.Entity<Employee>().HasMany(e => e.Claims)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

        builder.Entity<Employee>().HasMany(e => e.Logins)
            .WithOne()
            .HasForeignKey(ul => ul.UserId)
            .IsRequired();

        builder.Entity<Employee>().HasMany(e => e.Tokens)
            .WithOne()
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

        builder.Entity<Employee>().HasMany(e => e.EmployeePermissions)
            .WithOne(d => d.Employee)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<Permission>().ToTable("_Permissions");

        builder.Entity<Permission>().HasMany(e => e.EmployeePermissions)
            .WithOne(e => e.Permission)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<EmployeePermission>().ToTable("_EmployeePermissions");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("_EmployeeClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("_EmployeeLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("_EmployeeTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("_PermissionClaims");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("host=localhost;port=5432;Username=postgres;Password=***;Database=RE");
        //optionsBuilder.UseNpgsql("host=83.222.17.62;port=5432;Username=postgres;Password=LapinBoss2022!;Database=RE");
    }
}
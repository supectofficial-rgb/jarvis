namespace Insurance.UserService.Infra.Persistence.RDB.Queries;

using Insurance.UserService.Infra.Persistence.RDB.Queries.Organizations.Entities;
using Insurance.UserService.Infra.Persistence.RDB.Queries.Permissions.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InsuranceUserServiceQueryDbContext : QueryDbContext
{
    public InsuranceUserServiceQueryDbContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
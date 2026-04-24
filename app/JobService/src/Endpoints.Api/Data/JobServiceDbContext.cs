namespace Insurance.JobService.Endpoints.Api.Data;

using Hangfire.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class JobServiceDbContext : DbContext
{
    protected JobServiceDbContext() { }
    public JobServiceDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.OnHangfireModelCreating();
    }
}
namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Queries;

using Insurance.DynamicStructureService.Infra.Persistence.RDB.Queries.DynamicStructures.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class DynamicStructureServiceQueryDbContext : QueryDbContext
{
    public DynamicStructureServiceQueryDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<FormReadModel> Forms => Set<FormReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DynamicStructureServiceQueryDbContext).Assembly);
    }
}

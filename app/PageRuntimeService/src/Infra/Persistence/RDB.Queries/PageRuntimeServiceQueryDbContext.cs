namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries;

using Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class PageRuntimeServiceQueryDbContext : QueryDbContext
{
    public PageRuntimeServiceQueryDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<LanguageReadModel> Languages => Set<LanguageReadModel>();
    public DbSet<SectionReadModel> Sections => Set<SectionReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PageRuntimeServiceQueryDbContext).Assembly);
    }
}

namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Commands;

using Insurance.PageRuntimeService.AppCore.Domain.Languages.Entities;
using Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class PageRuntimeServiceCommandDbContext : CommandDbContext
{
    public PageRuntimeServiceCommandDbContext(DbContextOptions<PageRuntimeServiceCommandDbContext> options)
        : base(options)
    {
    }

    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Section> Sections => Set<Section>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Language>().ToTable("Languages");
        modelBuilder.Entity<Section>().ToTable("Sections");
        modelBuilder.Entity<Section>().HasIndex(x => new { x.Url, x.LanguageId }).IsUnique();
    }
}


namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Commands;

using Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;
using Insurance.DynamicStructureService.AppCore.Domain.FormTypes.Entities;
using Insurance.DynamicStructureService.AppCore.Domain.OrderOptions.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class DynamicStructureServiceCommandDbContext : CommandDbContext
{
    public DynamicStructureServiceCommandDbContext(DbContextOptions<DynamicStructureServiceCommandDbContext> options)
        : base(options)
    {
    }

    public DbSet<FormType> FormTypes => Set<FormType>();
    public DbSet<Form> Forms => Set<Form>();
    public DbSet<FormItem> FormItems => Set<FormItem>();
    public DbSet<OrderOption> OrderOptions => Set<OrderOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FormType>().ToTable("FormTypes");
        modelBuilder.Entity<Form>().ToTable("Forms");
        modelBuilder.Entity<FormItem>().ToTable("FormItems");
        modelBuilder.Entity<OrderOption>().ToTable("OrderOptions");

        modelBuilder.Entity<Form>()
            .HasMany(x => x.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}


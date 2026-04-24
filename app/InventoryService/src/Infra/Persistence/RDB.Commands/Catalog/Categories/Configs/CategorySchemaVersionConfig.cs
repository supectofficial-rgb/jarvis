namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategorySchemaVersionConfig : IEntityTypeConfiguration<CategorySchemaVersion>
{
    public void Configure(EntityTypeBuilder<CategorySchemaVersion> builder)
    {
        builder.ToTable("CategorySchemaVersions");
        builder.HasIndex(x => new { x.CategoryRef, x.VersionNo }).IsUnique();
        builder.HasIndex(x => new { x.CategoryRef, x.IsCurrent });

        builder.HasMany(x => x.Rules)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

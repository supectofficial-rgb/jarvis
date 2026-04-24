namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategorySchemaVersionReadModelConfig : IEntityTypeConfiguration<CategorySchemaVersionReadModel>
{
    public void Configure(EntityTypeBuilder<CategorySchemaVersionReadModel> builder)
    {
        builder.ToTable("CategorySchemaVersions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.CategoryRef, x.VersionNo }).IsUnique();
        builder.HasIndex(x => new { x.CategoryRef, x.IsCurrent });
    }
}

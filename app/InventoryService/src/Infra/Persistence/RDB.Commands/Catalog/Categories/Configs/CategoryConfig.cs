namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Ignore(x => x.AttributeRules);
        builder.Ignore(x => x.CurrentSchemaVersionRef);

        builder.HasMany(x => x.SchemaVersions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

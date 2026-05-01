namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.BusinessKey)
            .HasConversion(
                key => key.Value,
                value => BusinessKey.FromGuid(value));

        builder.Ignore(x => x.AttributeRules);
        builder.Ignore(x => x.CurrentSchemaVersionRef);

        builder.HasMany(x => x.SchemaVersions)
            .WithOne()
            .HasPrincipalKey(x => x.BusinessKey)
            .HasForeignKey(x => x.CategoryRef)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

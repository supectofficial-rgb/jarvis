namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategoryAttributeRuleConfig : IEntityTypeConfiguration<CategoryAttributeRule>
{
    public void Configure(EntityTypeBuilder<CategoryAttributeRule> builder)
    {
        builder.ToTable("CategoryAttributeRules");
        builder.Property(x => x.CategorySchemaVersionRef)
            .HasConversion(
                key => key.Value,
                value => BusinessKey.FromGuid(value));
        builder.HasIndex(x => new { x.CategorySchemaVersionRef, x.AttributeRef }).IsUnique();
        builder.HasIndex(x => x.CategorySchemaVersionRef);
    }
}

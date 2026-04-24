namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategoryAttributeRuleReadModelConfig : IEntityTypeConfiguration<CategoryAttributeRuleReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryAttributeRuleReadModel> builder)
    {
        builder.ToTable("CategoryAttributeRules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.CategorySchemaVersionRef, x.AttributeRef }).IsUnique();
        builder.HasIndex(x => x.CategorySchemaVersionRef);
    }
}

namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategoryVariantNameFormulaPartReadModelConfig : IEntityTypeConfiguration<CategoryVariantNameFormulaPartReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryVariantNameFormulaPartReadModel> builder)
    {
        builder.ToTable("CategoryVariantNameFormulaParts");
        builder.Property(x => x.Separator).HasMaxLength(16);
        builder.HasIndex(x => new { x.FormulaRef, x.SortOrder }).IsUnique();
        builder.HasIndex(x => new { x.FormulaRef, x.AttributeRef }).IsUnique();
    }
}

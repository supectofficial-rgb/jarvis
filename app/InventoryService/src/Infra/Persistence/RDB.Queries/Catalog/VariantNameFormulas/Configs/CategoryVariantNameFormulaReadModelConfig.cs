namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategoryVariantNameFormulaReadModelConfig : IEntityTypeConfiguration<CategoryVariantNameFormulaReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryVariantNameFormulaReadModel> builder)
    {
        builder.ToTable("CategoryVariantNameFormulas");
        builder.HasIndex(x => new { x.CategoryRef, x.Name }).IsUnique();
        builder.Property(x => x.IncludeCategoryName).HasDefaultValue(true);
    }
}

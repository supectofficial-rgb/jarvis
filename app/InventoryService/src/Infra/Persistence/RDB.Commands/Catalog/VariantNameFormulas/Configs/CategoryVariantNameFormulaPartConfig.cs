namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.VariantNameFormulas.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategoryVariantNameFormulaPartConfig : IEntityTypeConfiguration<CategoryVariantNameFormulaPart>
{
    public void Configure(EntityTypeBuilder<CategoryVariantNameFormulaPart> builder)
    {
        builder.ToTable("CategoryVariantNameFormulaParts");
        builder.HasIndex(x => new { x.FormulaRef, x.SortOrder }).IsUnique();
        builder.HasIndex(x => new { x.FormulaRef, x.AttributeRef }).IsUnique();
    }
}

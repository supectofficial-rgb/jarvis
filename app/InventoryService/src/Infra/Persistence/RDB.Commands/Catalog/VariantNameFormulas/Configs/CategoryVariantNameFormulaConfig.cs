namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.VariantNameFormulas.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategoryVariantNameFormulaConfig : IEntityTypeConfiguration<CategoryVariantNameFormula>
{
    public void Configure(EntityTypeBuilder<CategoryVariantNameFormula> builder)
    {
        builder.ToTable("CategoryVariantNameFormulas");
        builder.HasIndex(x => new { x.CategoryRef, x.Name }).IsUnique();
        builder.Property(x => x.BusinessKey)
            .HasConversion(
                key => key.Value,
                value => BusinessKey.FromGuid(value));

        builder.Property(x => x.Name).HasMaxLength(160);
        builder.Property(x => x.Separator).HasMaxLength(16);
        builder.Property(x => x.IncludeCategoryName).HasDefaultValue(true);

        builder.HasMany(x => x.Parts)
            .WithOne()
            .HasPrincipalKey(x => x.BusinessKey)
            .HasForeignKey(x => x.FormulaRef)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

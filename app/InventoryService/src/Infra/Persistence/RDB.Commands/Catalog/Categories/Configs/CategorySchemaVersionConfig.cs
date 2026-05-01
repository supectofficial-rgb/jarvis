namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategorySchemaVersionConfig : IEntityTypeConfiguration<CategorySchemaVersion>
{
    public void Configure(EntityTypeBuilder<CategorySchemaVersion> builder)
    {
        builder.ToTable("CategorySchemaVersions");
        builder.Property(x => x.CategoryRef)
            .HasConversion(
                key => key.Value,
                value => BusinessKey.FromGuid(value));

        builder.HasIndex(x => new { x.CategoryRef, x.VersionNo }).IsUnique();
        builder.HasIndex(x => new { x.CategoryRef, x.IsCurrent });

        builder.HasMany(x => x.Rules)
            .WithOne()
            .HasPrincipalKey(x => x.BusinessKey)
            .HasForeignKey(x => x.CategorySchemaVersionRef)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

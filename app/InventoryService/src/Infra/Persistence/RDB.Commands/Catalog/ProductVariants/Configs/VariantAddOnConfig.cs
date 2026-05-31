namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class VariantAddOnConfig : IEntityTypeConfiguration<VariantAddOn>
{
    public void Configure(EntityTypeBuilder<VariantAddOn> builder)
    {
        builder.ToTable("VariantAddOns");
        builder.Property(x => x.VariantRef)
            .HasConversion(
                key => key.Value,
                value => BusinessKey.FromGuid(value))
            .IsRequired();
        builder.Property(x => x.AddOnVariantRef).IsRequired(false);
        builder.Property(x => x.TagId).IsRequired(false);
        builder.Property(x => x.IsRequired).IsRequired();

        builder.HasCheckConstraint(
            "CK_VariantAddOns_AddOnVariantRefOrTagId",
            "\"AddOnVariantRef\" IS NOT NULL OR \"TagId\" IS NOT NULL");

        builder.HasIndex(x => new { x.VariantRef, x.AddOnVariantRef })
            .IsUnique()
            .HasFilter("\"AddOnVariantRef\" IS NOT NULL");

        builder.HasIndex(x => new { x.VariantRef, x.TagId })
            .IsUnique()
            .HasFilter("\"TagId\" IS NOT NULL");

        builder.HasOne<ProductVariant>()
            .WithMany(x => x.AddOns)
            .HasForeignKey(x => x.VariantRef)
            .HasPrincipalKey(x => x.BusinessKey)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

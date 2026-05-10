namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantImageReadModelConfig : IEntityTypeConfiguration<VariantImageReadModel>
{
    public void Configure(EntityTypeBuilder<VariantImageReadModel> builder)
    {
        builder.ToTable("VariantImages");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.VariantRef, x.FileKey }).IsUnique();
        builder.Property(x => x.FileKey).HasMaxLength(256);
        builder.Property(x => x.OriginalFileName).HasMaxLength(260);
        builder.Property(x => x.ContentType).HasMaxLength(128);
        builder.Property(x => x.OriginalUrl).HasMaxLength(1024);
        builder.Property(x => x.ThumbnailUrl).HasMaxLength(1024);
    }
}

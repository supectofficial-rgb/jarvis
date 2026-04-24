namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SellerReadModelConfig : IEntityTypeConfiguration<SellerReadModel>
{
    public void Configure(EntityTypeBuilder<SellerReadModel> builder)
    {
        builder.ToTable("Sellers");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

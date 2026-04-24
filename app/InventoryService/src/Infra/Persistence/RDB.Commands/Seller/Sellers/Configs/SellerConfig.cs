namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Seller.Sellers.Configs;

using Insurance.InventoryService.AppCore.Domain.Seller.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SellerConfig : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("Sellers");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

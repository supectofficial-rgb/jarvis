namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.PriceTypes.Configs;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PriceTypeConfig : IEntityTypeConfiguration<PriceType>
{
    public void Configure(EntityTypeBuilder<PriceType> builder)
    {
        builder.ToTable("PriceTypes");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FulfillmentReadModelConfig : IEntityTypeConfiguration<FulfillmentReadModel>
{
    public void Configure(EntityTypeBuilder<FulfillmentReadModel> builder)
    {
        builder.ToTable("Fulfillments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}

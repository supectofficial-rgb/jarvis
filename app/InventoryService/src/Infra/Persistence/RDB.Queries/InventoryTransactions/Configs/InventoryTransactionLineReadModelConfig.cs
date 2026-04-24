namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryTransactionLineReadModelConfig : IEntityTypeConfiguration<InventoryTransactionLineReadModel>
{
    public void Configure(EntityTypeBuilder<InventoryTransactionLineReadModel> builder)
    {
        builder.ToTable("InventoryTransactionLines");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.InventoryTransactionId);
        builder.HasIndex(x => x.VariantRef);
    }
}

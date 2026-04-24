namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryTransactionReadModelConfig : IEntityTypeConfiguration<InventoryTransactionReadModel>
{
    public void Configure(EntityTypeBuilder<InventoryTransactionReadModel> builder)
    {
        builder.ToTable("InventoryTransactions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.TransactionNo).IsUnique();
    }
}

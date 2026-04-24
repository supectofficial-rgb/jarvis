namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryTransactions.Configs;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryTransactionConfig : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");
        builder.HasIndex(x => x.TransactionNo).IsUnique();

        builder.HasMany(x => x.Lines)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

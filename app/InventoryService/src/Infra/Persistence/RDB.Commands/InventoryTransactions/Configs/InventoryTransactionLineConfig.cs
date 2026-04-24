namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryTransactions.Configs;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryTransactionLineConfig : IEntityTypeConfiguration<InventoryTransactionLine>
{
    public void Configure(EntityTypeBuilder<InventoryTransactionLine> builder)
    {
        builder.ToTable("InventoryTransactionLines");
    }
}

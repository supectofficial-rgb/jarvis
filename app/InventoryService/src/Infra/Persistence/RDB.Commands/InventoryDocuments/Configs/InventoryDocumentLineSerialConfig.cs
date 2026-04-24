namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryDocuments.Configs;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryDocumentLineSerialConfig : IEntityTypeConfiguration<InventoryDocumentLineSerial>
{
    public void Configure(EntityTypeBuilder<InventoryDocumentLineSerial> builder)
    {
        builder.ToTable("InventoryDocumentLineSerials");
    }
}

namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryDocumentLineSerialConfig : IEntityTypeConfiguration<InventoryDocumentLineSerialReadModel>
{
    public void Configure(EntityTypeBuilder<InventoryDocumentLineSerialReadModel> builder)
    {
        builder.ToTable("InventoryDocumentLineSerials");
        builder.HasKey(x => x.Id);
    }
}

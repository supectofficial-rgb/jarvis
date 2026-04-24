namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryDocumentReadModelConfig : IEntityTypeConfiguration<InventoryDocumentReadModel>
{
    public void Configure(EntityTypeBuilder<InventoryDocumentReadModel> builder)
    {
        builder.ToTable("InventoryDocuments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.DocumentNo).IsUnique();
    }
}

namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryDocumentLineReadModelConfig : IEntityTypeConfiguration<InventoryDocumentLineReadModel>
{
    public void Configure(EntityTypeBuilder<InventoryDocumentLineReadModel> builder)
    {
        builder.ToTable("InventoryDocumentLines");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.InventoryDocumentId);
        builder.HasIndex(x => x.VariantRef);
    }
}

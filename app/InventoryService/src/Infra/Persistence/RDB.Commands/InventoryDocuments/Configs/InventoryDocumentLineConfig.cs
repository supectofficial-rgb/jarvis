namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryDocuments.Configs;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryDocumentLineConfig : IEntityTypeConfiguration<InventoryDocumentLine>
{
    public void Configure(EntityTypeBuilder<InventoryDocumentLine> builder)
    {
        builder.ToTable("InventoryDocumentLines");

        builder.HasMany(x => x.Serials)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

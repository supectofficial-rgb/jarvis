namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryDocuments.Configs;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryDocumentConfig : IEntityTypeConfiguration<InventoryDocument>
{
    public void Configure(EntityTypeBuilder<InventoryDocument> builder)
    {
        builder.ToTable("InventoryDocuments");
        builder.HasIndex(x => x.DocumentNo).IsUnique();
        builder.Property(x => x.ExternalReferenceNo).HasMaxLength(120);

        builder.HasMany(x => x.Lines)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

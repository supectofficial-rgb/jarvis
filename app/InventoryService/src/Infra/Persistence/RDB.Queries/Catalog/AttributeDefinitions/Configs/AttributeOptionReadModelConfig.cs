namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AttributeOptionReadModelConfig : IEntityTypeConfiguration<AttributeOptionReadModel>
{
    public void Configure(EntityTypeBuilder<AttributeOptionReadModel> builder)
    {
        builder.ToTable("AttributeOptions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.AttributeRef, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.AttributeRef, x.Value }).IsUnique();
    }
}

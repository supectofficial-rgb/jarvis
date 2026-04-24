namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.AttributeDefinitions.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AttributeOptionConfig : IEntityTypeConfiguration<AttributeOption>
{
    public void Configure(EntityTypeBuilder<AttributeOption> builder)
    {
        builder.ToTable("AttributeOptions");
        builder.HasIndex(x => new { x.AttributeRef, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.AttributeRef, x.Value }).IsUnique();
    }
}

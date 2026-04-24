namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.AttributeDefinitions.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AttributeDefinitionConfig : IEntityTypeConfiguration<AttributeDefinition>
{
    public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
    {
        builder.ToTable("AttributeDefinitions");
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Options)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

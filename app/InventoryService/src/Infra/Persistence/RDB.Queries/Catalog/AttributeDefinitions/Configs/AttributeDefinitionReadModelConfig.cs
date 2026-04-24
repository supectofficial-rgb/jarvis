namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AttributeDefinitionReadModelConfig : IEntityTypeConfiguration<AttributeDefinitionReadModel>
{
    public void Configure(EntityTypeBuilder<AttributeDefinitionReadModel> builder)
    {
        builder.ToTable("AttributeDefinitions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

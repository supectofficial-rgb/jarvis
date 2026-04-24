namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CategoryReadModelConfig : IEntityTypeConfiguration<CategoryReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryReadModel> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

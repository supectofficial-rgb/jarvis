namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventorySourceAllocationReadModelConfig : IEntityTypeConfiguration<InventorySourceAllocationReadModel>
{
    public void Configure(EntityTypeBuilder<InventorySourceAllocationReadModel> builder)
    {
        builder.ToTable("InventorySourceAllocations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}

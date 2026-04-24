namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ReturnRequestReadModelConfig : IEntityTypeConfiguration<ReturnRequestReadModel>
{
    public void Configure(EntityTypeBuilder<ReturnRequestReadModel> builder)
    {
        builder.ToTable("ReturnRequests");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}

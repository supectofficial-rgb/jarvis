namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Queries.DynamicStructures.Configs;

using Insurance.DynamicStructureService.Infra.Persistence.RDB.Queries.DynamicStructures.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FormReadModelConfig : IEntityTypeConfiguration<FormReadModel>
{
    public void Configure(EntityTypeBuilder<FormReadModel> builder)
    {
        builder.ToTable("Forms");
        builder.HasKey(x => x.Id);
    }
}

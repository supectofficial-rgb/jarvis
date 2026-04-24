namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Configs;

using Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SectionReadModelConfig : IEntityTypeConfiguration<SectionReadModel>
{
    public void Configure(EntityTypeBuilder<SectionReadModel> builder)
    {
        builder.ToTable("Sections");
        builder.HasKey(x => x.Id);
    }
}

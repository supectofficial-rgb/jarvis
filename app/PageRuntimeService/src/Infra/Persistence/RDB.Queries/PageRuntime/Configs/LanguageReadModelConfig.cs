namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Configs;

using Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LanguageReadModelConfig : IEntityTypeConfiguration<LanguageReadModel>
{
    public void Configure(EntityTypeBuilder<LanguageReadModel> builder)
    {
        builder.ToTable("Languages");
        builder.HasKey(x => x.Id);
    }
}

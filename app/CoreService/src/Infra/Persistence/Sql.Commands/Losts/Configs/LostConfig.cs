namespace Insurance.Infra.Persistence.Sql.Commands.Losts.Configs;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LostConfig : IEntityTypeConfiguration<Lost>
{
    public void Configure(EntityTypeBuilder<Lost> builder)
    {
        builder.ToTable("Losts");
    }
}
namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Organizations.Configs;

using Insurance.UserService.Infra.Persistence.RDB.Queries.Organizations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrganizationConfig : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
    }
}
namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Permissions.Configs;

using Insurance.UserService.Infra.Persistence.RDB.Queries.Permissions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
    }
}
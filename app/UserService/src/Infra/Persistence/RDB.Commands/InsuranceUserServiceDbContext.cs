namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.MembershipRoleAssignments.Entities;
using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Extensions;
using Insurance.UserService.Infra.Persistence.RDB.Commands.ValueConversions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands.Interceptors;
using OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;
using System.Globalization;

public class InsuranceUserServiceDbContext : IdentityDbContext<Account, AppRole, long>
{
    public InsuranceUserServiceDbContext(DbContextOptions<InsuranceUserServiceDbContext> options) : base(options) { }
    public InsuranceUserServiceDbContext() { }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MembershipRoleAssignment> MembershipRoleAssignments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }

    public T? GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
    {
        var value = Entry(entity).Property(propertyName).CurrentValue;
        return value != null ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture) : default;
    }

    public object GetShadowPropertyValue(object entity, string propertyName)
    {
        return Entry(entity).Property(propertyName).CurrentValue!;
    }

    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        var entityType = Model.FindEntityType(clrEntityType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var entityNavigations = new List<INavigation>();
            foreach (var navigation in entityType.GetNavigations())
            {
                if (includedNavigations.Add(navigation))
                    entityNavigations.Add(navigation);
            }

            if (entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in entityNavigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation != null)
                        includedNavigations.Add(inverseNavigation);
                }
                stack.Push(entityNavigations.GetEnumerator());
            }

            while (stack.Count > 0 && !stack.Peek().MoveNext())
                stack.Pop();

            if (stack.Count == 0) break;
            entityType = stack.Peek().Current.TargetEntityType;
        }
    }

    private void ConfigureRoleIndexes(ModelBuilder builder)
    {
        var role = builder.Entity<AppRole>();

        role.Metadata
            .GetIndexes()
            .Where(i => i.Properties.Any(p => p.Name == "NormalizedName"))
            .ToList()
            .ForEach(i => role.Metadata.RemoveIndex(i));

        var isPostgres = Database.ProviderName?.Contains("Npgsql") == true;

        var index = role
            .HasIndex(r => new { r.TenantId, r.NormalizedName })
            .IsUnique();

        if (isPostgres)
            index.HasFilter("\"NormalizedName\" IS NOT NULL");
        else
            index.HasFilter("[NormalizedName] IS NOT NULL");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureRoleIndexes(builder);

        builder.AddAuditableShadowProperties();

        builder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessKey)
                  .HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
                  .IsRequired();

            entity.Property(e => e.TenantId).HasConversion(new TenantIdConversion());
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpirationDate);

            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.ReasonRevoked).HasMaxLength(500);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
        });

        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.BusinessKey)
                  .HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
                  .IsRequired();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.BusinessKey)
                  .HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
                  .IsRequired();

            entity.Property(e => e.TenantId).HasConversion(new TenantIdConversion());

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.Property(e => e.BusinessKey)
                  .HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
                  .IsRequired();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<Membership>(entity =>
        {
            entity.ToTable("Memberships");
            entity.Property(e => e.TenantId).HasConversion(new TenantIdConversion());
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<MembershipRoleAssignment>(entity =>
        {
            entity.ToTable("UserRoleAssignments");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity
            .Property(e => e.BusinessKey)
            .HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
            .IsRequired();

            entity
.Property(e => e.RoleBusinessKey)
.HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
.IsRequired();

            entity
.Property(e => e.MembershipBusinessKey)
.HasConversion(v => v.Value, v => BusinessKey.FromGuid(v))
.IsRequired();
        });

        builder.Entity<OtpCode>(entity =>
        {
            entity.ToTable("OtpCodes");
            entity.Property(e => e.MobileNumber).HasMaxLength(32);
            entity.Property(e => e.Code).HasMaxLength(16).IsRequired();
            entity.HasIndex(e => e.MobileNumber);
            entity.HasIndex(e => e.CreatedAt);
        });

        //SeedData.Seed(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
                 .EnableSensitiveDataLogging()
                 .EnableDetailedErrors();

        optionsBuilder.AddInterceptors(new AddAuditDataInterceptor());

        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<BusinessKey>().HaveConversion<BusinessKeyConversion>();
    }
}

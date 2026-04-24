namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.Infra.Auth.UserServices;

public static class AuditableShadowProperties
{
    public static readonly Func<object, string> EFPropertyCreatedByUserId =
                                    entity => EF.Property<string>(entity, CreatedByUserId);
    public static readonly string CreatedByUserId = nameof(CreatedByUserId);

    public static readonly Func<object, string> EFPropertyModifiedByUserId =
                                    entity => EF.Property<string>(entity, ModifiedByUserId);
    public static readonly string ModifiedByUserId = nameof(ModifiedByUserId);

    public static readonly Func<object, DateTime?> EFPropertyCreatedDateTime =
                                    entity => EF.Property<DateTime?>(entity, CreatedDateTime);
    public static readonly string CreatedDateTime = nameof(CreatedDateTime);

    public static readonly Func<object, DateTime?> EFPropertyModifiedDateTime =
                                    entity => EF.Property<DateTime?>(entity, ModifiedDateTime);
    public static readonly string ModifiedDateTime = nameof(ModifiedDateTime);

    public static readonly string CreatedByOrganizationBusinessKey = nameof(CreatedByOrganizationBusinessKey);
    public static readonly string ModifiedByOrganizationBusinessKey = nameof(ModifiedByOrganizationBusinessKey);
    public static readonly string CreatedByPersonaBusinessKey = nameof(CreatedByPersonaBusinessKey);
    public static readonly string ModifiedByPersonaBusinessKey = nameof(ModifiedByPersonaBusinessKey);

    public static void AddAuditableShadowProperties(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(c => typeof(IAuditableEntity).IsAssignableFrom(c.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(CreatedByUserId).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(ModifiedByUserId).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime?>(CreatedDateTime);
            modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime?>(ModifiedDateTime);

            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(CreatedByOrganizationBusinessKey).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(ModifiedByOrganizationBusinessKey).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(CreatedByPersonaBusinessKey).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(ModifiedByPersonaBusinessKey).HasMaxLength(50);
        }
    }

    public static void SetAuditableEntityPropertyValues(
        this ChangeTracker changeTracker,
        IUserInfoService userInfoService)
    {

        var now = DateTime.UtcNow;
        var userId = userInfoService.UserIdOrDefault();

        var organizationBusinessKey =
            userInfoService.GetClaim("activeOrganizationBusinessKey")
            ?? userInfoService.GetClaim("currentOrganizationKey");

        var personaBusinessKey =
            userInfoService.GetClaim("activePersonaBusinessKey")
            ?? userInfoService.GetClaim("activeMembershipBusinessKey")
            ?? userInfoService.GetClaim("currentMembershipKey")
            ?? userInfoService.GetClaim("personaKey");

        var modifiedEntries = changeTracker.Entries<IAuditableEntity>().Where(x => x.State == EntityState.Modified);
        foreach (var modifiedEntry in modifiedEntries)
        {
            modifiedEntry.Property(ModifiedDateTime).CurrentValue = now;
            modifiedEntry.Property(ModifiedByUserId).CurrentValue = userId;

            TrySetShadowProperty(modifiedEntry, ModifiedByOrganizationBusinessKey, organizationBusinessKey);
            TrySetShadowProperty(modifiedEntry, ModifiedByPersonaBusinessKey, personaBusinessKey);
        }

        var addedEntries = changeTracker.Entries<IAuditableEntity>().Where(x => x.State == EntityState.Added);
        foreach (var addedEntry in addedEntries)
        {
            addedEntry.Property(CreatedDateTime).CurrentValue = now;
            addedEntry.Property(CreatedByUserId).CurrentValue = userId;

            TrySetShadowProperty(addedEntry, CreatedByOrganizationBusinessKey, organizationBusinessKey);
            TrySetShadowProperty(addedEntry, CreatedByPersonaBusinessKey, personaBusinessKey);
        }
    }

    private static void TrySetShadowProperty(EntityEntry entry, string propertyName, object? value)
    {
        if (entry.Metadata.FindProperty(propertyName) is null)
            return;

        entry.Property(propertyName).CurrentValue = value;
    }
}

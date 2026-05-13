namespace OysterFx.Infra.Persistence.RDB.Commands.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using OysterFx.Infra.Auth.UserServices;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;
using System.Linq.Expressions;

public static class OrganizationShadowProperties
{
    public static readonly string OrganizationBusinessKey = nameof(OrganizationBusinessKey);

    public static void AddOrganizationShadowProperties(
        this ModelBuilder modelBuilder,
        Expression<Func<string?>> currentOrganizationBusinessKeyExpression)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(ShouldApplyTo))
        {
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(OrganizationBusinessKey)
                        .HasMaxLength(50);

            modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(OrganizationBusinessKey);

            modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(BuildOrganizationFilter(entityType.ClrType, currentOrganizationBusinessKeyExpression));
        }
    }

    public static void SetOrganizationPropertyValues(
        this ChangeTracker changeTracker,
        IUserInfoService userInfoService)
    {
        var organizationBusinessKey = userInfoService.GetActiveOrganizationBusinessKey();

        var tenantEntries = changeTracker
            .Entries()
            .Where(x => x.Metadata.FindProperty(OrganizationBusinessKey) is not null)
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (tenantEntries.Count == 0)
            return;

        if (string.IsNullOrWhiteSpace(organizationBusinessKey))
            throw new InvalidOperationException("Active organization claim is required to save tenant-scoped records.");

        foreach (var entry in tenantEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(OrganizationBusinessKey).CurrentValue = organizationBusinessKey;
                continue;
            }

            if (entry.State == EntityState.Modified && string.IsNullOrWhiteSpace(entry.Property(OrganizationBusinessKey).CurrentValue as string))
                entry.Property(OrganizationBusinessKey).CurrentValue = organizationBusinessKey;
        }
    }

    public static string? GetActiveOrganizationBusinessKey(this IUserInfoService? userInfoService)
    {
        if (userInfoService is null)
            return null;

        return userInfoService.GetClaim("activeOrganizationBusinessKey")
               ?? userInfoService.GetClaim("currentOrganizationKey")
               ?? userInfoService.GetClaim("organizationBusinessKey")
               ?? userInfoService.GetClaim("OrganizationBusinessKey")
               ?? userInfoService.GetClaim("activeOrganizationId")
               ?? userInfoService.GetClaim("organizationId")
               ?? userInfoService.GetClaim("OrganizationId");
    }

    private static bool ShouldApplyTo(IMutableEntityType entityType)
        => !entityType.IsOwned()
           && entityType.ClrType is not null
           && entityType.ClrType != typeof(OutboxEvent)
           && entityType.FindPrimaryKey() is not null;

    private static LambdaExpression BuildOrganizationFilter(
        Type clrType,
        Expression<Func<string?>> currentOrganizationBusinessKeyExpression)
    {
        var entityParameter = Expression.Parameter(clrType, "entity");

        var entityOrganization = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(string) },
            entityParameter,
            Expression.Constant(OrganizationBusinessKey));

        var currentOrganization = currentOrganizationBusinessKeyExpression.Body;

        var tenantIsResolved = Expression.Not(Expression.Call(
            typeof(string),
            nameof(string.IsNullOrWhiteSpace),
            Type.EmptyTypes,
            currentOrganization));

        var belongsToCurrentTenant = Expression.Equal(entityOrganization, currentOrganization);
        var body = Expression.AndAlso(tenantIsResolved, belongsToCurrentTenant);

        return Expression.Lambda(body, entityParameter);
    }
}

namespace OysterFx.Infra.Persistence.RDB.Commands.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Auth.UserServices;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;
using System.Linq.Expressions;

public static class OrganizationShadowProperties
{
    public static readonly string OrganizationBusinessKey = nameof(OrganizationBusinessKey);
    public static readonly string TenantId = nameof(TenantId);

    public static void AddOrganizationShadowProperties(
        this ModelBuilder modelBuilder,
        Expression<Func<string?>> currentOrganizationBusinessKeyExpression,
        Expression<Func<string?>> currentTenantIdExpression)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(ShouldApplyTo))
        {
            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(OrganizationBusinessKey)
                        .HasMaxLength(50);

            modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(OrganizationBusinessKey);

            modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(TenantId)
                        .HasMaxLength(100);

            modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(TenantId);

            modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(BuildOrganizationFilter(entityType.ClrType, currentOrganizationBusinessKeyExpression, currentTenantIdExpression));
        }
    }

    public static void SetOrganizationPropertyValues(
        this ChangeTracker changeTracker,
        IUserInfoService userInfoService)
    {
        var organizationBusinessKey = userInfoService.GetActiveOrganizationBusinessKey();
        var tenantId = userInfoService.GetActiveTenantId();

        var tenantEntries = changeTracker
            .Entries()
            .Where(x => x.Metadata.FindProperty(OrganizationBusinessKey) is not null && x.Metadata.FindProperty(TenantId) is not null)
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (tenantEntries.Count == 0)
            return;

        if (string.IsNullOrWhiteSpace(organizationBusinessKey))
            throw new InvalidOperationException("Active organization claim is required to save tenant-scoped records.");

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("Active tenant claim is required to save tenant-scoped records.");

        foreach (var entry in tenantEntries)
        {
            if (entry.State == EntityState.Added)
            {
                SetOrganizationPropertyValue(entry, OrganizationBusinessKey, organizationBusinessKey);
                SetOrganizationPropertyValue(entry, TenantId, tenantId);
                continue;
            }

            if (entry.State == EntityState.Modified && IsNullOrWhiteSpace(entry.Property(OrganizationBusinessKey).CurrentValue))
                SetOrganizationPropertyValue(entry, OrganizationBusinessKey, organizationBusinessKey);

            if (entry.State == EntityState.Modified && IsNullOrWhiteSpace(entry.Property(TenantId).CurrentValue))
                SetOrganizationPropertyValue(entry, TenantId, tenantId);
        }
    }

    private static void SetOrganizationPropertyValue(EntityEntry entry, string propertyName, string? value)
    {
        var property = entry.Metadata.FindProperty(propertyName);
        if (property is null || string.IsNullOrWhiteSpace(value))
            return;

        if (IsBusinessKeyType(property.ClrType))
        {
            entry.Property(propertyName).CurrentValue = BusinessKey.FromGuid(Guid.Parse(value));
            return;
        }

        entry.Property(propertyName).CurrentValue = value;
    }

    private static bool IsBusinessKeyType(Type type)
        => string.Equals(type.FullName, typeof(BusinessKey).FullName, StringComparison.Ordinal);

    private static bool IsNullOrWhiteSpace(object? value)
        => value is null || string.IsNullOrWhiteSpace(value as string);

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

    public static string? GetActiveTenantId(this IUserInfoService? userInfoService)
    {
        if (userInfoService is null)
            return null;

        return userInfoService.GetClaim("activeTenantId")
               ?? userInfoService.GetClaim("currentTenantId")
               ?? userInfoService.GetClaim("tenantId")
               ?? userInfoService.GetClaim("TenantId");
    }

    private static bool ShouldApplyTo(IMutableEntityType entityType)
        => !entityType.IsOwned()
           && entityType.ClrType is not null
           && entityType.ClrType != typeof(OutboxEvent)
           && entityType.FindPrimaryKey() is not null;

    private static LambdaExpression BuildOrganizationFilter(
        Type clrType,
        Expression<Func<string?>> currentOrganizationBusinessKeyExpression,
        Expression<Func<string?>> currentTenantIdExpression)
    {
        var entityParameter = Expression.Parameter(clrType, "entity");

        var entityOrganization = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(string) },
            entityParameter,
            Expression.Constant(OrganizationBusinessKey));

        var currentOrganization = currentOrganizationBusinessKeyExpression.Body;
        var entityTenant = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(string) },
            entityParameter,
            Expression.Constant(TenantId));
        var currentTenant = currentTenantIdExpression.Body;

        var organizationIsResolved = Expression.Not(Expression.Call(
            typeof(string),
            nameof(string.IsNullOrWhiteSpace),
            Type.EmptyTypes,
            currentOrganization));

        var tenantIsResolved = Expression.Not(Expression.Call(
            typeof(string),
            nameof(string.IsNullOrWhiteSpace),
            Type.EmptyTypes,
            currentTenant));

        var belongsToCurrentOrganization = Expression.Equal(entityOrganization, currentOrganization);
        var belongsToCurrentTenant = Expression.Equal(entityTenant, currentTenant);
        var body = Expression.AndAlso(
            Expression.AndAlso(organizationIsResolved, tenantIsResolved),
            Expression.AndAlso(belongsToCurrentOrganization, belongsToCurrentTenant));

        return Expression.Lambda(body, entityParameter);
    }
}

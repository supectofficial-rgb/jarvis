namespace OysterFx.Infra.Persistence.RDB.Queries.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OysterFx.Infra.Auth.UserServices;
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

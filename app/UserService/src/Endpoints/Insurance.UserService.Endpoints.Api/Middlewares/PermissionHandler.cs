namespace Insurance.UserService.Endpoints.Api.Middlewares;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.AspNetCore.Authorization;
using OysterFx.Infra.Auth.UserServices.Policies.Services;
using System.Security.Claims;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(
        ICacheServiceCaller cacheServiceCaller,
        IHttpContextAccessor httpContextAccessor)
    {
        _cacheServiceCaller = cacheServiceCaller;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return;
        }

        var permissionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var activeRoleClaims = user
            .FindAll("activeRoleBusinessKey")
            .Select(c => c.Value)
            .Where(v => Guid.TryParse(v, out _))
            .Distinct()
            .ToList();

        foreach (var roleBusinessKey in activeRoleClaims)
        {
            var cacheKey = PermissionCacheKeys.ForRolePermissions(Guid.Parse(roleBusinessKey));
            var permissionsCsv = await _cacheServiceCaller.GetAsync(new(cacheKey));

            if (permissionsCsv.Error is not null && permissionsCsv.Error.Any())
                continue;

            if (string.IsNullOrWhiteSpace(permissionsCsv.Success.Value))
                continue;

            foreach (var permission in permissionsCsv.Success.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                permissionSet.Add(permission);
            }
        }

        // Legacy fallback for old tokens without role-based claims
        if (!permissionSet.Any())
        {
            var userIdClaim = user.FindFirst("businessKey")?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(userIdClaim))
            {
                var legacyCacheKey = $"user--permissions--{userIdClaim}";
                var permissionsCsv = await _cacheServiceCaller.GetAsync(new(legacyCacheKey));

                if (permissionsCsv.Error is null || !permissionsCsv.Error.Any())
                {
                    foreach (var permission in permissionsCsv.Success.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    {
                        permissionSet.Add(permission);
                    }
                }
            }
        }

        if (permissionSet.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

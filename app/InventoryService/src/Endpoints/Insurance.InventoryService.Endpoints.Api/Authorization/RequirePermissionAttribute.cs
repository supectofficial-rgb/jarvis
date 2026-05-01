namespace Insurance.InventoryService.Endpoints.Api.Authorization;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
        : base(typeof(RequirePermissionFilter))
    {
        Arguments = new object[] { permissions };
    }

    private sealed class RequirePermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly ICacheServiceCaller _cacheServiceCaller;
        private readonly string[] _permissions;

        public RequirePermissionFilter(ICacheServiceCaller cacheServiceCaller, string[] permissions)
        {
            _cacheServiceCaller = cacheServiceCaller;
            _permissions = permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_permissions.Length == 0)
            {
                return;
            }

            var roles = user.Claims
                .Where(claim =>
                    string.Equals(claim.Type, "role", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(claim.Type, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.OrdinalIgnoreCase))
                .Select(claim => claim.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .SelectMany(value => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (roles.Contains("Admin") || roles.Contains("SysAdmin"))
            {
                return;
            }

            var roleBusinessKeys = user
                .FindAll("activeRoleBusinessKey")
                .Select(claim => claim.Value)
                .Concat(user.FindFirst("activeRoleBusinessKeys")?.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>())
                .Where(value => Guid.TryParse(value, out _))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (roleBusinessKeys.Count == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            var permissionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var roleBusinessKey in roleBusinessKeys)
            {
                var permissionsCsv = await _cacheServiceCaller.GetAsync(
                    new GetFromCacheRequest($"role--permissions--{roleBusinessKey}"));

                if (permissionsCsv.Error is { Count: > 0 } || string.IsNullOrWhiteSpace(permissionsCsv.Success.Value))
                    continue;

                foreach (var permission in permissionsCsv.Success.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    permissionSet.Add(permission);
                }
            }

            if (permissionSet.Contains("*") || _permissions.Any(permissionSet.Contains))
            {
                return;
            }

            context.Result = new ForbidResult();
        }
    }
}

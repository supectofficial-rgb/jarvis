using Insurance.CacheService.Infra.CallerService.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Insurance.CoreService.Endpoints.Api.Extensions.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

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

            var userIdClaim = user.FindFirst("UserBusinessKey")?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Fail();
                return;
            }

            // دریافت دسترسی‌های کاربر از Redis
            var cacheKey = $"user--permissions--{userIdClaim}";
            var permissionsCsv = await _cacheServiceCaller.GetAsync(new(cacheKey));

            if (permissionsCsv.Error is not null && permissionsCsv.Error.Any())
            {
                context.Fail();
                return;
            }

            var permissions = permissionsCsv.Success.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

}

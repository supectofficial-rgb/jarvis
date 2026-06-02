namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OysterFx.AppCore.Domain.ValueObjects;

public class LoginCompletionService : ILoginCompletionService
{
    private readonly ITokenService _tokenService;
    private readonly IAuthSessionService _authSessionService;
    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly InsuranceUserServiceDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public LoginCompletionService(
        ITokenService tokenService,
        IAuthSessionService authSessionService,
        ICacheServiceCaller cacheServiceCaller,
        InsuranceUserServiceDbContext dbContext,
        IOptions<JwtSettings> jwtSettings)
    {
        _tokenService = tokenService;
        _authSessionService = authSessionService;
        _cacheServiceCaller = cacheServiceCaller;
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(LoginByCredentialCommandResult? Result, string? Error)> CompleteAsync(Account user)
    {
        var systemRoleKeys = await _dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(
                _dbContext.Roles.Where(r => r.Scope == RoleScope.System),
                ur => ur.RoleId,
                role => role.Id,
                (ur, role) => role.BusinessKey)
            .Distinct()
            .ToListAsync();

        var memberships = await _dbContext.Memberships
            .Where(m => m.UserBusinessKey == user.UserBusinessKey && m.IsActive)
            .OrderBy(m => m.Id)
            .Select(m => new
            {
                m.BusinessKey,
                m.OrganizationBusinessKey,
                TenantId = m.TenantId.Value
            })
            .ToListAsync();

        var membershipKeys = memberships
            .Select(m => m.BusinessKey)
            .Distinct()
            .ToList();

        var membershipRolePairs = await _dbContext.MembershipRoleAssignments
            .Where(ra => membershipKeys.Contains(ra.MembershipBusinessKey))
            .Select(ra => new
            {
                ra.MembershipBusinessKey,
                ra.RoleBusinessKey
            })
            .ToListAsync();

        if (!memberships.Any() && !systemRoleKeys.Any())
        {
            return (null, "کاربر هیچ نقش فعالی برای ورود ندارد");
        }

        var membershipDtos = membershipRolePairs
            .Join(
                memberships,
                pair => pair.MembershipBusinessKey,
                membership => membership.BusinessKey,
                (pair, membership) => new MembershipDto
                {
                    BusinessKey = pair.MembershipBusinessKey,
                    OrganizationBusinessKey = membership.OrganizationBusinessKey,
                    TenantId = membership.TenantId,
                    RoleBusinessKey = pair.RoleBusinessKey
                })
            .ToList();

        var activeMembership = memberships.FirstOrDefault();

        var activeMembershipRoleKeys = activeMembership == null
            ? new List<BusinessKey>()
            : membershipRolePairs
                .Where(pair => pair.MembershipBusinessKey == activeMembership.BusinessKey)
                .Select(pair => pair.RoleBusinessKey)
                .Distinct()
                .ToList();

        var activeRoleKeys = activeMembershipRoleKeys
            .Concat(systemRoleKeys)
            .Distinct()
            .ToList();

        var allRoleKeys = membershipRolePairs
            .Select(pair => pair.RoleBusinessKey)
            .Concat(systemRoleKeys)
            .Distinct()
            .ToList();

        var activeRoleNames = await _dbContext.Roles
            .Where(role => activeRoleKeys.Contains(role.BusinessKey) && role.Name != null)
            .Select(role => role.Name!)
            .Distinct()
            .ToListAsync();

        var rolePermissionPairs = await _dbContext.RolePermissions
            .Where(rp => allRoleKeys.Contains(rp.RoleBusinessKey))
            .Join(
                _dbContext.Permissions.Where(p => p.IsActive),
                rp => rp.PermissionBusinessKey,
                p => p.BusinessKey,
                (rp, p) => new
                {
                    rp.RoleBusinessKey,
                    p.Code
                })
            .Distinct()
            .ToListAsync();

        var rolePermissionMap = rolePermissionPairs
            .GroupBy(x => x.RoleBusinessKey)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Code).Distinct().ToList());

        foreach (var roleKey in allRoleKeys)
        {
            var rolePermissions = rolePermissionMap.TryGetValue(roleKey, out var codes)
                ? codes
                : new List<string>();

            await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
                Key: PermissionCacheKeys.ForRolePermissions(roleKey.Value),
                Value: string.Join(',', rolePermissions),
                AbsoluteExpirationMinutes: 60
            ));
        }

        await _cacheServiceCaller.RemoveAsync(new RemoveFromCacheRequest(
            Key: PermissionCacheKeys.ForUserPermissions(user.UserBusinessKey.Value)));

        var sessionId = Guid.NewGuid().ToString("N");
        var tokenResult = await _tokenService.GenerateTokenAsync(
            user,
            membershipDtos,
            sessionId,
            activeMembership?.BusinessKey,
            activeMembership?.OrganizationBusinessKey,
            activeMembership?.TenantId,
            activeRoleKeys,
            activeRoleNames);

        var authSession = new AuthSession
        {
            SessionId = sessionId,
            UserId = user.Id,
            UserBusinessKey = user.UserBusinessKey,
            RefreshTokenHash = _authSessionService.HashRefreshToken(tokenResult.RefreshToken),
            CreatedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        await _authSessionService.CreateAsync(
            authSession,
            (int)TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays).TotalMinutes);

        var activePermissionCodes = activeRoleKeys
            .SelectMany(roleKey => rolePermissionMap.TryGetValue(roleKey, out var codes)
                ? codes
                : Enumerable.Empty<string>())
            .Distinct()
            .ToList();

        var activeMembershipBusinessKey = GetBusinessKeyValue(activeMembership?.BusinessKey);
        var activeOrganizationBusinessKey = GetBusinessKeyValue(activeMembership?.OrganizationBusinessKey);
        var activeRoleBusinessKeys = activeRoleKeys
            .Where(role => role is not null)
            .Select(role => role!.Value)
            .ToList();

        var result = new LoginByCredentialCommandResult
        {
            Token = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            TokenExpiration = tokenResult.Expiration,
            User = user,
            Memberships = membershipDtos,
            Permissions = activePermissionCodes,
            Roles = activeRoleNames,
            ActiveMembershipBusinessKey = activeMembershipBusinessKey,
            ActiveOrganizationBusinessKey = activeOrganizationBusinessKey,
            ActiveTenantId = activeMembership?.TenantId,
            ActiveRoleBusinessKeys = activeRoleBusinessKeys
        };

        return (result, null);
    }

    private static Guid? GetBusinessKeyValue(BusinessKey? businessKey)
        => businessKey is null ? null : businessKey.Value;
}

namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OysterFx.AppCore.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly InsuranceUserServiceDbContext _context;
    private readonly IAuthSessionService _authSessionService;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        InsuranceUserServiceDbContext context,
        IAuthSessionService authSessionService,
        ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _authSessionService = authSessionService;
        _logger = logger;
    }

    public async Task<TokenResult> GenerateTokenAsync(
        Account user,
        IEnumerable<MembershipDto> memberships,
        string sessionId,
        BusinessKey? activeMembershipKey,
        BusinessKey? activeOrganizationKey,
        string? activeTenantId,
        IEnumerable<BusinessKey> activeRoleBusinessKeys,
        IEnumerable<string>? activeRoleNames = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("username", user.UserName ?? ""),
            new Claim("businessKey", user.UserBusinessKey.Value.ToString()),
            new Claim("sessionId", sessionId),
            new Claim("sid", sessionId)
        };

        if (activeMembershipKey is not null)
        {
            claims.Add(new Claim("currentMembershipKey", activeMembershipKey.Value.ToString()));
            claims.Add(new Claim("activeMembershipBusinessKey", activeMembershipKey.Value.ToString()));
        }

        if (activeOrganizationKey is not null)
        {
            claims.Add(new Claim("currentOrganizationKey", activeOrganizationKey.Value.ToString()));
            claims.Add(new Claim("activeOrganizationBusinessKey", activeOrganizationKey.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(activeTenantId))
        {
            claims.Add(new Claim("currentTenantId", activeTenantId));
            claims.Add(new Claim("activeTenantId", activeTenantId));
        }

        var normalizedActiveRoleKeys = activeRoleBusinessKeys?
            .Select(role => role.Value)
            .Distinct()
            .ToList() ?? new List<Guid>();

        foreach (var roleBusinessKey in normalizedActiveRoleKeys)
        {
            claims.Add(new Claim("activeRoleBusinessKey", roleBusinessKey.ToString()));
        }

        if (normalizedActiveRoleKeys.Any())
        {
            claims.Add(new Claim("activeRoleBusinessKeys", string.Join(',', normalizedActiveRoleKeys)));
        }

        var normalizedActiveRoleNames = activeRoleNames?
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? new List<string>();

        foreach (var roleName in normalizedActiveRoleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
            claims.Add(new Claim("role", roleName));
        }

        if (memberships != null)
        {
            var membershipsJson = memberships
                .GroupBy(m => new
                {
                    MembershipBusinessKey = m.BusinessKey.Value,
                    OrganizationBusinessKey = m.OrganizationBusinessKey.Value,
                    m.TenantId
                })
                .Select(g => new
                {
                    g.Key.MembershipBusinessKey,
                    g.Key.OrganizationBusinessKey,
                    g.Key.TenantId,
                    RoleBusinessKeys = g
                        .Select(x => x.RoleBusinessKey.Value)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            var membershipsJsonStr = System.Text.Json.JsonSerializer.Serialize(membershipsJson);
            claims.Add(new Claim("memberships", membershipsJsonStr));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expires
        };
    }

    public async Task<TokenResult> GenerateTokenWithMembershipAsync(Account user, BusinessKey membershipKey)
    {
        //var membership = await _context.UserMemberships
        //    .Include(m => m.Organization)
        //    .FirstOrDefaultAsync(m => m.BusinessKey == membershipKey);

        //if (membership == null)
        //    throw new InvalidOperationException("Membership not found");

        //return await GenerateTokenAsync(user, new[] { membership });

        return default!;
    }

    private async Task<string> GenerateRefreshTokenAsync(long userId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<TokenResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
            throw new SecurityTokenException("Invalid access token");

        if (!long.TryParse(principal.FindFirst("userId")?.Value, out var userId) || userId <= 0)
            throw new SecurityTokenException("Invalid access token");

        var sessionId = principal.FindFirst("sessionId")?.Value ?? principal.FindFirst("sid")?.Value;
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new SecurityTokenException("Session not found in access token");

        var storedSession = await _authSessionService.GetAsync(sessionId);
        if (storedSession == null || storedSession.IsRevoked || storedSession.ExpiresAt < DateTime.UtcNow)
            throw new SecurityTokenException("Session expired");

        if (!string.Equals(storedSession.UserId.ToString(), userId.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            await _authSessionService.RevokeAsync(sessionId, "Session user mismatch");
            throw new SecurityTokenException("Session user mismatch");
        }

        if (!_authSessionService.VerifyRefreshToken(refreshToken, storedSession.RefreshTokenHash))
        {
            await _authSessionService.RevokeAsync(sessionId, "Invalid refresh token");
            throw new SecurityTokenException("Invalid refresh token");
        }

        var storedRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpirationDate < DateTime.UtcNow)
        {
            await _authSessionService.RevokeAsync(sessionId, "Invalid refresh token");
            throw new SecurityTokenException("Invalid refresh token");
        }

        var user = await _context.Set<Account>().FindAsync(userId);
        if (user == null)
            throw new SecurityTokenException("User not found");

        var memberships = await _context.Memberships
            .Where(m => m.UserBusinessKey == user.UserBusinessKey && m.IsActive)
            .OrderBy(m => m.Id)
            .Select(m => new
            {
                m.BusinessKey,
                m.OrganizationBusinessKey,
                TenantId = m.TenantId.Value
            })
            .ToListAsync();

        var membershipKeys = memberships.Select(m => m.BusinessKey).Distinct().ToList();

        var membershipRolePairs = await _context.MembershipRoleAssignments
            .Where(ra => membershipKeys.Contains(ra.MembershipBusinessKey))
            .Select(ra => new
            {
                ra.MembershipBusinessKey,
                ra.RoleBusinessKey
            })
            .ToListAsync();

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

        var activeMembershipKey = TryGetBusinessKeyClaim(principal, "currentMembershipKey")
            ?? memberships.Select(m => m.BusinessKey).FirstOrDefault();

        var activeOrganizationKey = TryGetBusinessKeyClaim(principal, "currentOrganizationKey")
            ?? memberships.Select(m => m.OrganizationBusinessKey).FirstOrDefault();

        var activeTenantId = principal.FindFirst("currentTenantId")?.Value;
        var activeRoleBusinessKeys = GetRoleBusinessKeys(principal);
        var activeRoleNames = GetRoleNames(principal);

        var tokenResult = await GenerateTokenAsync(
            user,
            membershipDtos,
            sessionId,
            activeMembershipKey,
            activeOrganizationKey,
            activeTenantId,
            activeRoleBusinessKeys,
            activeRoleNames);

        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.ReasonRevoked = "Replaced by new token";
        storedRefreshToken.ReplacedByToken = tokenResult.RefreshToken;
        await _context.SaveChangesAsync();

        storedSession.RefreshTokenHash = _authSessionService.HashRefreshToken(tokenResult.RefreshToken);
        storedSession.LastActivityAt = DateTime.UtcNow;
        storedSession.ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        var sessionUpdated = await _authSessionService.UpdateAsync(
            storedSession,
            (int)TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays).TotalMinutes);

        if (!sessionUpdated)
        {
            _logger.LogWarning("Auth session update failed for session {SessionId}", sessionId);
        }

        return tokenResult;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedRefreshToken == null)
            return false;

        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.ReasonRevoked = "Manually revoked";
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeSessionAsync(string accessToken, string refreshToken, string? reason = null)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
            return false;

        if (!long.TryParse(principal.FindFirst("userId")?.Value, out var userId) || userId <= 0)
            return false;

        var sessionId = principal.FindFirst("sessionId")?.Value ?? principal.FindFirst("sid")?.Value;
        var revokeReason = string.IsNullOrWhiteSpace(reason) ? "Logged out" : reason;
        var sessionRevoked = false;

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            var session = await _authSessionService.GetAsync(sessionId);
            if (session is not null)
            {
                session.IsRevoked = true;
                session.RevokedAt = DateTime.UtcNow;
                session.RevokedReason = revokeReason;
                session.LastActivityAt = DateTime.UtcNow;
                sessionRevoked = await _authSessionService.UpdateAsync(
                    session,
                    (int)TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays).TotalMinutes);
            }
            else
            {
                sessionRevoked = await _authSessionService.RevokeAsync(sessionId, revokeReason);
            }
        }

        var storedRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken is not null && !storedRefreshToken.IsRevoked)
        {
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.ReasonRevoked = revokeReason;
            await _context.SaveChangesAsync();
        }

        return sessionRevoked || storedRefreshToken is not null;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null) return false;

            var userId = principal.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId)) return false;

            var user = await _context.Set<Account>().FindAsync(long.Parse(userId));
            return user != null;
        }
        catch
        {
            return false;
        }
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, parameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return null;
        }
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Expired token validation failed");
            return null;
        }
    }

    private static BusinessKey? TryGetBusinessKeyClaim(ClaimsPrincipal principal, string claimType)
    {
        var value = principal.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var guid) ? BusinessKey.FromGuid(guid) : null;
    }

    private static IEnumerable<BusinessKey> GetRoleBusinessKeys(ClaimsPrincipal principal)
    {
        var values = principal.FindAll("activeRoleBusinessKey")
            .Select(claim => claim.Value)
            .Concat((principal.FindFirst("activeRoleBusinessKeys")?.Value ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var value in values)
        {
            if (Guid.TryParse(value, out var guid))
            {
                yield return BusinessKey.FromGuid(guid);
            }
        }
    }

    private static IEnumerable<string> GetRoleNames(ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Concat(principal.FindAll("role").Select(claim => claim.Value))
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<BusinessKey?> GetCurrentMembershipFromTokenAsync(string token)
    {
        var principal = GetPrincipalFromToken(token);
        var membershipKeyValue = principal?.FindFirst("currentMembershipKey")?.Value;

        if (string.IsNullOrEmpty(membershipKeyValue))
            return null;

        return BusinessKey.FromGuid(Guid.Parse(membershipKeyValue));
    }
}

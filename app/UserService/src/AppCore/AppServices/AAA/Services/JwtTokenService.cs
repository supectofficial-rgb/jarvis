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
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        InsuranceUserServiceDbContext context,
        ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _logger = logger;
    }

    public async Task<TokenResult> GenerateTokenAsync(
        Account user,
        IEnumerable<MembershipDto> memberships,
        BusinessKey? activeMembershipKey,
        BusinessKey? activeOrganizationKey,
        IEnumerable<BusinessKey> activeRoleBusinessKeys)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("username", user.UserName ?? ""),
            new Claim("businessKey", user.UserBusinessKey.Value.ToString())
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

        if (memberships != null)
        {
            var membershipsJson = memberships
                .GroupBy(m => new
                {
                    MembershipBusinessKey = m.BusinessKey.Value,
                    OrganizationBusinessKey = m.OrganizationBusinessKey.Value
                })
                .Select(g => new
                {
                    g.Key.MembershipBusinessKey,
                    g.Key.OrganizationBusinessKey,
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
        var principal = GetPrincipalFromToken(accessToken);
        if (principal == null)
            throw new SecurityTokenException("Invalid access token");

        var userId = long.Parse(principal.FindFirst("userId")?.Value ?? "0");

        var storedRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpirationDate < DateTime.UtcNow)
            throw new SecurityTokenException("Invalid refresh token");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new SecurityTokenException("User not found");

        // دریافت Membershipهای کاربر
        //var memberships = await _context.UserMemberships
        //    .Include(m => m.Organization)
        //    .Where(m => m.UserBusinessKey == user.BusinessKey)
        //    .ToListAsync();

        //storedRefreshToken.IsRevoked = true;
        //storedRefreshToken.ReasonRevoked = "Replaced by new token";
        //await _context.SaveChangesAsync();

        //return await GenerateTokenAsync(user, memberships);
        return default!;
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

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null) return false;

            var userId = principal.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId)) return false;

            var user = await _context.Users.FindAsync(long.Parse(userId));
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

    public async Task<BusinessKey?> GetCurrentMembershipFromTokenAsync(string token)
    {
        var principal = GetPrincipalFromToken(token);
        var membershipKeyValue = principal?.FindFirst("currentMembershipKey")?.Value;

        if (string.IsNullOrEmpty(membershipKeyValue))
            return null;

        return BusinessKey.FromGuid(Guid.Parse(membershipKeyValue));
    }
}

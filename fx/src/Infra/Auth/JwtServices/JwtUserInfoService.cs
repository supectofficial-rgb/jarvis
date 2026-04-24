namespace OysterFx.Infra.Auth.JwtServices;

using OysterFx.Infra.Auth.UserServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class JwtUserInfoService : IUserInfoService
{
    private readonly Dictionary<string, string> _claims = new(StringComparer.OrdinalIgnoreCase);

    public string? UserId { get; private set; }
    public string? Username { get; private set; }

    public string? GetClaim(string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return null;

        return _claims.TryGetValue(claimType, out var value)
            ? value
            : null;
    }

    public string GetFirstName() => GetClaim(ClaimTypes.GivenName) ?? string.Empty;

    public string GetLastName() => GetClaim(ClaimTypes.Surname) ?? string.Empty;

    public string GetUserAgent() => string.Empty;

    public string GetUserId() => UserId ?? string.Empty;

    public string GetUserIp() => string.Empty;

    public string GetUsername() => Username ?? string.Empty;

    public bool IsCurrentUser(string userId) => !string.IsNullOrWhiteSpace(userId) && userId == UserId;

    public void LoadFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                ResetUserInfo();
                return;
            }

            token = token.Trim().Replace("\"", string.Empty);

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                ResetUserInfo();
                return;
            }

            var jwtToken = handler.ReadJwtToken(token);
            ExtractUserInfoFromToken(jwtToken);
        }
        catch
        {
            ResetUserInfo();
        }
    }

    private void ExtractUserInfoFromToken(JwtSecurityToken jwtToken)
    {
        ResetUserInfo();

        foreach (var claim in jwtToken.Claims)
        {
            if (string.IsNullOrWhiteSpace(claim.Type))
                continue;

            _claims[claim.Type] = claim.Value;
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
            c.Type == "userId" ||
            c.Type == "UserId" ||
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == JwtRegisteredClaimNames.Sub ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        UserId = userIdClaim?.Value;

        var usernameClaim = jwtToken.Claims.FirstOrDefault(c =>
            c.Type == "username" ||
            c.Type == ClaimTypes.Name ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

        Username = usernameClaim?.Value;
    }

    private void ResetUserInfo()
    {
        _claims.Clear();
        UserId = null;
        Username = null;
    }

    public string UserIdOrDefault() => !string.IsNullOrWhiteSpace(UserId) ? UserId : string.Empty;

    public string UserIdOrDefault(string defaultValue) => !string.IsNullOrWhiteSpace(UserId) ? UserId : defaultValue;
}

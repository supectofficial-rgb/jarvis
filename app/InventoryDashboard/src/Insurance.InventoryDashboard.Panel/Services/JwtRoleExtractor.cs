using System.Text;
using System.Text.Json;

namespace Insurance.InventoryDashboard.Panel.Services;

public static class JwtRoleExtractor
{
    private static readonly string[] RoleClaimNames =
    [
        "role",
        "roles",
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"
    ];

    public static IReadOnlyList<string> ExtractRoles(string? jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
        {
            return Array.Empty<string>();
        }

        var tokenParts = jwtToken.Split('.');
        if (tokenParts.Length < 2)
        {
            return Array.Empty<string>();
        }

        var payload = Base64UrlDecode(tokenParts[1]);
        if (string.IsNullOrWhiteSpace(payload))
        {
            return Array.Empty<string>();
        }

        try
        {
            using var document = JsonDocument.Parse(payload);
            var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var claimName in RoleClaimNames)
            {
                if (!document.RootElement.TryGetProperty(claimName, out var claimValue))
                {
                    continue;
                }

                switch (claimValue.ValueKind)
                {
                    case JsonValueKind.String:
                    {
                        var role = claimValue.GetString();
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            roles.Add(role.Trim());
                        }

                        break;
                    }
                    case JsonValueKind.Array:
                        foreach (var item in claimValue.EnumerateArray())
                        {
                            if (item.ValueKind != JsonValueKind.String)
                            {
                                continue;
                            }

                            var role = item.GetString();
                            if (!string.IsNullOrWhiteSpace(role))
                            {
                                roles.Add(role.Trim());
                            }
                        }

                        break;
                }
            }

            return roles.ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private static string Base64UrlDecode(string encoded)
    {
        var normalized = encoded.Replace('-', '+').Replace('_', '/');
        var paddingLength = 4 - normalized.Length % 4;
        if (paddingLength is > 0 and < 4)
        {
            normalized = normalized.PadRight(normalized.Length + paddingLength, '=');
        }

        try
        {
            var bytes = Convert.FromBase64String(normalized);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}

namespace Insurance.UserService.AppCore.Domain.Users.Dtos;

using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public class UserContext
{
    public TenantId TenantId { get; private set; }
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public BusinessKey UserBusinessKey { get; set; } = null!;

    public BusinessKey? CurrentOrganizationBusinessKey { get; set; }
    public BusinessKey? CurrentPersonaBusinessKey { get; set; }

    /// <summary>
    /// عضویت فعلی کاربر در یک سازمان (Tenant) و نقش
    /// </summary>
    public Membership? CurrentMembership { get; set; }

    /// <summary>
    /// تمام عضویت‌های کاربر در سازمان‌ها و نقش‌ها
    /// </summary>
    public List<Membership> AvailableMemberships { get; set; } = new();

    /// <summary>
    /// مجوزهای فعلی کاربر
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// سازمان فعلی (Tenant)
    /// </summary>
    public Organization? CurrentOrganization { get; set; }

    /// <summary>
    /// تمام سازمان‌هایی که کاربر عضو آن‌ها است
    /// </summary>
    public List<Organization> Organizations { get; set; } = new();

    public bool IsAuthenticated { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public bool HasPermission(string permissionCode)
    {
        return Permissions.Contains(permissionCode);
    }

    public bool HasAnyPermission(params string[] permissionCodes)
    {
        return permissionCodes.Any(p => Permissions.Contains(p));
    }

    public bool HasAllPermissions(params string[] permissionCodes)
    {
        return permissionCodes.All(p => Permissions.Contains(p));
    }
}

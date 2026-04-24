namespace Insurance.UserService.AppCore.Domain.Memberships.Entities;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public class Membership : AggregateRoot
{
    public TenantId TenantId { get; private set; }
    public BusinessKey UserBusinessKey { get; private set; }
    public BusinessKey OrganizationBusinessKey { get; private set; }

    public DateTime StartDateTime { get; private set; }
    public bool IsActive { get; private set; }

    private Membership() { }
    private Membership(
        TenantId tenantId,
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey)
    {
        TenantId = tenantId;
        UserBusinessKey = userBusinessKey;
        OrganizationBusinessKey = organizationBusinessKey;
        StartDateTime = DateTime.UtcNow;
        IsActive = true;
    }

    public static Membership Create(
        TenantId tenantId,
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey) => new(tenantId, userBusinessKey, organizationBusinessKey);
}
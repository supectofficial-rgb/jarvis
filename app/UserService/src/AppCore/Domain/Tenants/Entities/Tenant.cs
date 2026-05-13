namespace Insurance.UserService.AppCore.Domain.Tenants.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class Tenant : AggregateRoot
{
    public TenantId TenantId { get; private set; } = null!;
    public BusinessKey OrganizationBusinessKey { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Tenant() { }

    private Tenant(TenantId tenantId, BusinessKey organizationBusinessKey, string name)
    {
        TenantId = tenantId;
        OrganizationBusinessKey = organizationBusinessKey;
        Name = name.Trim();
        IsActive = true;
    }

    public static Tenant Create(TenantId tenantId, BusinessKey organizationBusinessKey, string name)
        => new(tenantId, organizationBusinessKey, name);
}

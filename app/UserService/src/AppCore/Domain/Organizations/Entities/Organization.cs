namespace Insurance.UserService.AppCore.Domain.Organizations.Entities;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using OysterFx.AppCore.Domain.Aggregates;

public class Organization : AggregateRoot
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    private Organization() { }

    private Organization(string name)
    {
        Name = name;
        IsActive = true;
        TenantId = TenantId.New();
    }

    public static Organization NewOrganization(string name) => new(name);

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
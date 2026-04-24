namespace Insurance.UserService.AppCore.Domain.Tenants.Events;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

public record OrganizationCreatedEvent : IDomainEvent
{
    public BusinessKey OrganizationBusinessKey { get; }
    public string Name { get; }
    public TenantId TenantId { get; }
    public DateTime OccurredOn { get; }

    public OrganizationCreatedEvent(BusinessKey organizationBusinessKey, string name, TenantId tenantId)
    {
        OrganizationBusinessKey = organizationBusinessKey;
        Name = name;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}

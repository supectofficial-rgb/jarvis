namespace Insurance.UserService.AppCore.Domain.Tenants.Events;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

public record UserCreatedEvent : IDomainEvent
{
    public BusinessKey UserBusinessKey { get; }
    public string Email { get; }
    public TenantId TenantId { get; }
    public DateTime OccurredOn { get; }

    public UserCreatedEvent(BusinessKey userBusinessKey, string email, TenantId tenantId)
    {
        UserBusinessKey = userBusinessKey;
        Email = email;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}
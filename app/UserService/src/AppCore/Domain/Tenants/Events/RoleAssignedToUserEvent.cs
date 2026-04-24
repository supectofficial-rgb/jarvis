namespace Insurance.UserService.AppCore.Domain.Tenants.Events;

using Insurance.UserService.AppCore.Domain.Roles.Enums;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

public record RoleAssignedToUserEvent : IDomainEvent
{
    public BusinessKey UserBusinessKey { get; }
    public BusinessKey RoleBusinessKey { get; }
    public BusinessKey? PersonaBusinessKey { get; }
    public RoleScope RoleScope { get; }
    public DateTime OccurredOn { get; }

    public RoleAssignedToUserEvent(
        BusinessKey userBusinessKey,
        BusinessKey roleBusinessKey,
        RoleScope roleScope,
        BusinessKey? personaBusinessKey = null)
    {
        UserBusinessKey = userBusinessKey;
        RoleBusinessKey = roleBusinessKey;
        RoleScope = roleScope;
        PersonaBusinessKey = personaBusinessKey;
        OccurredOn = DateTime.UtcNow;
    }
}
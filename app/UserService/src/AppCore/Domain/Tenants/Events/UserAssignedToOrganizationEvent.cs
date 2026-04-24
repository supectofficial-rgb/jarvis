namespace Insurance.UserService.AppCore.Domain.Tenants.Events;

using Insurance.UserService.AppCore.Domain.Tenants.Enums;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

public record UserAssignedToOrganizationEvent : IDomainEvent
{
    public BusinessKey UserBusinessKey { get; }
    public BusinessKey OrganizationBusinessKey { get; }
    public AssignmentType AssignmentType { get; }
    public BusinessKey? PersonaBusinessKey { get; }
    public DateTime OccurredOn { get; }

    public UserAssignedToOrganizationEvent(
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey,
        AssignmentType assignmentType,
        BusinessKey? personaBusinessKey = null)
    {
        UserBusinessKey = userBusinessKey;
        OrganizationBusinessKey = organizationBusinessKey;
        AssignmentType = assignmentType;
        PersonaBusinessKey = personaBusinessKey;
        OccurredOn = DateTime.UtcNow;
    }
}

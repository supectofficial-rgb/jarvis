namespace Insurance.UserService.AppCore.Domain.Tenants.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

public record PersonaCreatedEvent : IDomainEvent
{
    public BusinessKey PersonaBusinessKey { get; }
    public BusinessKey UserBusinessKey { get; }
    public BusinessKey OrganizationBusinessKey { get; }
    public string Name { get; }
    public DateTime OccurredOn { get; }

    public PersonaCreatedEvent(
        BusinessKey personaBusinessKey,
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey,
        string name)
    {
        PersonaBusinessKey = personaBusinessKey;
        UserBusinessKey = userBusinessKey;
        OrganizationBusinessKey = organizationBusinessKey;
        Name = name;
        OccurredOn = DateTime.UtcNow;
    }
}

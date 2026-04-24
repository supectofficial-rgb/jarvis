namespace OysterFx.AppCore.Shared.Events;

using OysterFx.AppCore.Domain.Events;

public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent Event);
}
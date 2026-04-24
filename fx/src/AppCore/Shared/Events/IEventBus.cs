namespace OysterFx.AppCore.Shared.Events;

using OysterFx.AppCore.Domain.Events;

public interface IEventBus
{
    Task PublishAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class, IDomainEvent;
}
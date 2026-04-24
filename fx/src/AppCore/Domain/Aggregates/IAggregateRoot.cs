namespace OysterFx.AppCore.Domain.Aggregates;

using OysterFx.AppCore.Domain.Events;

public interface IAggregateRoot
{
    void ClearEvents();
    IEnumerable<IDomainEvent> GetEvents();
}
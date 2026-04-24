namespace OysterFx.Infra.EventBus.Contract.Events;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredAtUtc { get; }
}
namespace OysterFx.Infra.EventBus.Contract.Events;

public sealed record EventEnvelope<T>(Guid EventId, string EventType, IDictionary<string, string> Metadata, T Payload) : IIntegrationEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
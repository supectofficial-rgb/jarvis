namespace OysterFx.Infra.EventBus.Abstractions;

using OysterFx.Infra.EventBus.Contract.Events;

public interface IEventBus
{
    Task PublishAsync<T>(EventEnvelope<T> envelope, CancellationToken ct);
    Task PublishRawAsync(Guid eventId, string eventType, string payload, IDictionary<string, string> metadata, CancellationToken ct);
    Task SubscribeAsync<T>(string eventType, Func<EventEnvelope<T>, Task> handler, CancellationToken ct);
}
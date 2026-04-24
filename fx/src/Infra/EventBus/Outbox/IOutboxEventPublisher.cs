namespace OysterFx.Infra.EventBus.Outbox;

using OysterFx.Infra.EventBus.Contract.Events;

public interface IOutboxEventPublisher
{
    Task PublishAsync<T>(EventEnvelope<T> envelope, CancellationToken ct);
    Task PublishAsync<T>(T payload, string? eventType = null, IDictionary<string, string>? metadata = null, CancellationToken ct = default);
}

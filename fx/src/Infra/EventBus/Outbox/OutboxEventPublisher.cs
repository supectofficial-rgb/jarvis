namespace OysterFx.Infra.EventBus.Outbox;

using OysterFx.Infra.EventBus.Contract.Events;
using System.Text.Json;

public sealed class OutboxEventPublisher : IOutboxEventPublisher
{
    private readonly IOutboxStore _outboxStore;

    public OutboxEventPublisher(IOutboxStore outboxStore)
    {
        _outboxStore = outboxStore;
    }

    public async Task PublishAsync<T>(EventEnvelope<T> envelope, CancellationToken ct)
    {
        var outbox = new OutboxMessage
        {
            EventType = IntegrationEventTypeResolver.Resolve(typeof(T), envelope.EventType),
            Payload = JsonSerializer.Serialize(envelope.Payload),
            Metadata = JsonSerializer.Serialize(envelope.Metadata ?? new Dictionary<string, string>()),
            OccurredAtUtc = envelope.OccurredAtUtc
        };

        await _outboxStore.AddAsync(outbox, ct);
        await _outboxStore.SaveChangesAsync(ct);
    }

    public Task PublishAsync<T>(
        T payload,
        string? eventType = null,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default)
    {
        var envelope = new EventEnvelope<T>(
            Guid.NewGuid(),
            IntegrationEventTypeResolver.Resolve<T>(eventType),
            metadata ?? new Dictionary<string, string>(),
            payload);

        return PublishAsync(envelope, ct);
    }
}

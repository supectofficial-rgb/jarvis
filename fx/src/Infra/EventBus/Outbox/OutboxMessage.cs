namespace OysterFx.Infra.EventBus.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string EventType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public string Metadata { get; init; } = "{}";
    public int RetryCount { get; set; }
    public bool Published { get; set; }
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
}

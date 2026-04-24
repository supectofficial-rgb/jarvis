namespace OysterFx.Infra.EventBus.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; init; }
    public Guid EventId { get; init; }
    public DateTime ProcessedAtUtc { get; init; }
}
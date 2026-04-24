namespace OysterFx.Infra.EventBus.Inbox;

public interface IInboxStore
{
    Task<bool> IsProcessedAsync(Guid eventId, CancellationToken ct);
    Task MarkProcessedAsync(Guid eventId, CancellationToken ct);
}
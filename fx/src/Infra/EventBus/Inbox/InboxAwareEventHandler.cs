namespace OysterFx.Infra.EventBus.Inbox;

using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;

public sealed class InboxAwareEventHandler<T>
{
    private readonly IInboxStore _inboxStore;
    private readonly IEventHandler<T> _inner;

    public InboxAwareEventHandler(
        IInboxStore inboxStore,
        IEventHandler<T> inner)
    {
        _inboxStore = inboxStore;
        _inner = inner;
    }

    public async Task HandleAsync(EventEnvelope<T> envelope, CancellationToken ct)
    {
        if (await _inboxStore.IsProcessedAsync(envelope.EventId, ct))
            return;

        await _inner.HandleAsync(envelope, ct);

        await _inboxStore.MarkProcessedAsync(envelope.EventId, ct);
    }
}
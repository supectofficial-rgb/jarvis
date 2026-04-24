namespace OysterFx.Infra.EventBus.Abstractions;

using OysterFx.Infra.EventBus.Contract.Events;

public interface IEventHandler<T>
{
    Task HandleAsync(EventEnvelope<T> envelope, CancellationToken ct);
}
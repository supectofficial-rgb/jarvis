namespace OysterFx.Infra.EventBus.RabbitMqBroker.Consumers.Convention;

using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;
using System.Reflection;

internal sealed class ConventionEventConsumerRegistrar : IEventConsumerRegistrar
{
    private readonly IEventBus _eventBus;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IReadOnlyList<EventConsumerRegistration> _registrations;

    public ConventionEventConsumerRegistrar(
        IEventBus eventBus,
        IServiceScopeFactory scopeFactory,
        IEnumerable<EventConsumerRegistration> registrations)
    {
        _eventBus = eventBus;
        _scopeFactory = scopeFactory;
        _registrations = registrations
            .DistinctBy(x => new { x.PayloadType, x.EventType })
            .ToList();
    }

    public void RegisterConsumers()
    {
        foreach (var registration in _registrations)
        {
            RegisterByPayloadType(registration.PayloadType, registration.EventType);
        }
    }

    private void RegisterByPayloadType(Type payloadType, string eventType)
    {
        var method = typeof(ConventionEventConsumerRegistrar)
            .GetMethod(nameof(RegisterByPayloadTypeGeneric), BindingFlags.Instance | BindingFlags.NonPublic)!;

        method.MakeGenericMethod(payloadType)
            .Invoke(this, new object[] { eventType });
    }

    private void RegisterByPayloadTypeGeneric<TPayload>(string eventType)
    {
        _eventBus.SubscribeAsync<TPayload>(
                eventType,
                envelope => DispatchToHandlers(envelope),
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    private async Task DispatchToHandlers<TPayload>(EventEnvelope<TPayload> envelope)
    {
        using var scope = _scopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TPayload>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(envelope, CancellationToken.None);
        }
    }
}

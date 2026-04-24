namespace OysterFx.AppCore.AppServices.Events;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Shared.Events;
using System.Diagnostics;

public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBus> _logger;

    public EventBus(IServiceProvider serviceProvider, ILogger<EventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class, IDomainEvent
    {
        var stopwatch = Stopwatch.StartNew();
        var counter = 0;

        try
        {
            _logger.LogDebug(
                "Routing event of type {EventType} with payload {Event}.",
                @event.GetType(),
                @event);

            var handlers = _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                counter++;
                tasks.Add(handler.Handle(@event));
            }

            await Task.WhenAll(tasks);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "No suitable handler found for event type {EventType}.",
                @event.GetType());
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogDebug(
                "Executed {HandlerCount} handlers for {EventType} in {ElapsedMs}ms.",
                counter,
                @event.GetType(),
                stopwatch.ElapsedMilliseconds);
        }
    }
}

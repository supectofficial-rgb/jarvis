namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;
using System.Reflection;
using System.Text.Json;
using DomainEventBus = OysterFx.AppCore.Shared.Events.IEventBus;

public sealed class DomainOutboxDispatcherBackgroundService : BackgroundService
{
    private static readonly MethodInfo PublishMethod = typeof(DomainEventBus)
        .GetMethods()
        .Single(x => x.Name == "PublishAsync" && x.IsGenericMethodDefinition && x.GetParameters().Length == 1);

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DomainOutboxDispatcherBackgroundService> _logger;
    private readonly DomainOutboxDispatcherOptions _options;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DomainOutboxDispatcherBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<DomainOutboxDispatcherOptions> options,
        ILogger<DomainOutboxDispatcherBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_options.Enabled)
                {
                    await Task.Delay(Math.Max(200, _options.PollingIntervalMs), stoppingToken);
                    continue;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxEventRepository>();
                var eventBus = scope.ServiceProvider.GetService<DomainEventBus>();

                if (outboxRepository is null || eventBus is null)
                {
                    await Task.Delay(Math.Max(200, _options.PollingIntervalMs), stoppingToken);
                    continue;
                }

                var events = outboxRepository.ReadEvents(Math.Max(1, _options.BatchSize)).ToList();
                if (events.Count == 0)
                {
                    await Task.Delay(Math.Max(200, _options.PollingIntervalMs), stoppingToken);
                    continue;
                }

                var processed = new List<OutboxEvent>(events.Count);

                foreach (var outboxEvent in events)
                {
                    if (string.IsNullOrWhiteSpace(outboxEvent.EventTypeName) || string.IsNullOrWhiteSpace(outboxEvent.EventPayload))
                    {
                        processed.Add(outboxEvent);
                        continue;
                    }

                    var eventType = ResolveType(outboxEvent.EventTypeName);
                    if (eventType is null)
                    {
                        _logger.LogWarning(
                            "Domain outbox event type {EventTypeName} was not found. EventId={EventId}",
                            outboxEvent.EventTypeName,
                            outboxEvent.EventId);

                        processed.Add(outboxEvent);
                        continue;
                    }

                    object? payload;
                    try
                    {
                        payload = JsonSerializer.Deserialize(outboxEvent.EventPayload, eventType, _jsonOptions);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Domain outbox payload deserialize failed. EventType={EventTypeName}, EventId={EventId}",
                            outboxEvent.EventTypeName,
                            outboxEvent.EventId);

                        processed.Add(outboxEvent);
                        continue;
                    }

                    if (payload is null)
                    {
                        processed.Add(outboxEvent);
                        continue;
                    }

                    try
                    {
                        var publishCall = PublishMethod.MakeGenericMethod(eventType).Invoke(eventBus, new[] { payload });
                        if (publishCall is Task publishTask)
                            await publishTask;

                        processed.Add(outboxEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Domain outbox publish failed. EventType={EventTypeName}, EventId={EventId}",
                            outboxEvent.EventTypeName,
                            outboxEvent.EventId);
                    }
                }

                if (processed.Count > 0)
                {
                    outboxRepository.MarkAsRead(processed);
                }

                await Task.Delay(Math.Max(200, _options.PollingIntervalMs), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Domain outbox dispatcher cycle failed.");
                await Task.Delay(Math.Max(1000, _options.ErrorIntervalMs), stoppingToken);
            }
        }
    }

    private static Type? ResolveType(string eventTypeName)
    {
        var direct = Type.GetType(eventTypeName, throwOnError: false);
        if (direct is not null)
            return direct;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var resolved = assembly.GetType(eventTypeName, throwOnError: false, ignoreCase: false);
            if (resolved is not null)
                return resolved;
        }

        return null;
    }
}

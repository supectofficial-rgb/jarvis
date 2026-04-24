namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OysterFx.Infra.EventBus.Abstractions;
using System.Text.Json;

public sealed class OutboxDispatcherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxDispatcherBackgroundService> _logger;
    private readonly OutboxDispatcherOptions _options;

    public OutboxDispatcherBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<OutboxDispatcherOptions> options,
        ILogger<OutboxDispatcherBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "OutboxDispatcherBackgroundService started for {ApplicationName}",
            _options.ApplicationName ?? "unknown");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_options.Enabled)
                {
                    await Task.Delay(Math.Max(200, _options.SendInterval), stoppingToken);
                    continue;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                var outboxStore = scope.ServiceProvider.GetService<IOutboxStore>();
                var eventBus = scope.ServiceProvider.GetService<IEventBus>();

                if (outboxStore is null || eventBus is null)
                {
                    await Task.Delay(Math.Max(200, _options.SendInterval), stoppingToken);
                    continue;
                }

                var take = Math.Max(1, _options.SendCount);
                var messages = await outboxStore.GetUnpublishedAsync(take, stoppingToken);

                if (messages.Count == 0)
                {
                    await Task.Delay(Math.Max(200, _options.SendInterval), stoppingToken);
                    continue;
                }

                foreach (var msg in messages)
                {
                    try
                    {
                        var metadata = DeserializeMetadata(msg.Metadata);

                        await eventBus.PublishRawAsync(
                            msg.Id,
                            msg.EventType,
                            msg.Payload,
                            metadata,
                            stoppingToken);

                        await outboxStore.MarkPublishedAsync(msg.Id, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Outbox publish failed for EventType={EventType}, MessageId={MessageId}",
                            msg.EventType,
                            msg.Id);
                    }
                }

                await Task.Delay(Math.Max(200, _options.SendInterval), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox dispatcher cycle failed");
                await Task.Delay(Math.Max(1000, _options.ExceptionInterval), stoppingToken);
            }
        }

        _logger.LogInformation(
            "OutboxDispatcherBackgroundService stopped for {ApplicationName}",
            _options.ApplicationName ?? "unknown");
    }

    private static IDictionary<string, string> DeserializeMetadata(string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(metadataJson))
            return new Dictionary<string, string>();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(metadataJson)
                ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}

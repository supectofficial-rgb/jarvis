namespace OysterFx.Infra.EventBus.RabbitMqBroker.Consumers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OysterFx.Infra.EventBus.RabbitMqBroker.Connections;

public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqConnection _rabbitMqConnection;

    public RabbitMqConsumerHostedService(IServiceProvider serviceProvider, IRabbitMqConnection rabbitMqConnection)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqConnection = rabbitMqConnection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConsumerRegistrationGuard.EnsureRegistered(_serviceProvider);
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
}

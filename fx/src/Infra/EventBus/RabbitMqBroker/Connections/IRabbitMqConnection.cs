namespace OysterFx.Infra.EventBus.RabbitMqBroker.Connections;

using RabbitMQ.Client;

public interface IRabbitMqConnection : IAsyncDisposable
{
    bool IsConnected { get; }
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);
}
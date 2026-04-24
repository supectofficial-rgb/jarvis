namespace OysterFx.Infra.EventBus.RabbitMqBroker.Connections;

using RabbitMQ.Client;

public class RabbitMqPersistentConnection : IRabbitMqConnection
{
    private readonly IConnectionFactory _factory;
    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public bool IsConnected => _connection is { IsOpen: true };

    public RabbitMqPersistentConnection(IConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (IsConnected) return true;

            _connection = await _factory.CreateConnectionAsync(cancellationToken);

            _connection.ConnectionShutdownAsync += async (sender, args) => await TryConnectAsync(cancellationToken);
            _connection.CallbackExceptionAsync += async (sender, args) => await TryConnectAsync(cancellationToken);
            _connection.ConnectionBlockedAsync += async (sender, args) => await TryConnectAsync(cancellationToken);

            return true;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
            throw new InvalidOperationException("RabbitMQ is not connected");

        return await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }
}
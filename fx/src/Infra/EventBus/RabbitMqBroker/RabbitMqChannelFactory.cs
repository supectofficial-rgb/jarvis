namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using RabbitMQ.Client;

public sealed class RabbitMqChannelFactory : IRabbitMqChannelFactory, IAsyncDisposable
{
    private readonly IConnection _connection;

    public RabbitMqChannelFactory(RabbitMqOptions options, CancellationToken ct = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            UserName = options.UserName,
            Password = options.Password,
            Port = options.Port
        };

        _connection = factory.CreateConnectionAsync(ct).GetAwaiter().GetResult();
    }

    public Task<IChannel> CreateAsync(CancellationToken ct = default)
    {
        return _connection.CreateChannelAsync(cancellationToken: ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}
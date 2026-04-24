namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using RabbitMQ.Client;

public interface IRabbitMqChannelFactory
{
    Task<IChannel> CreateAsync(CancellationToken ct = default);
}
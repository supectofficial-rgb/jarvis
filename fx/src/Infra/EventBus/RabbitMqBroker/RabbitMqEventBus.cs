namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;
using OysterFx.Infra.EventBus.RabbitMqBroker.Metrics;
using OysterFx.Infra.EventBus.RabbitMqBroker.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public sealed class RabbitMqEventBus : IEventBus
{
    private readonly IRabbitMqChannelFactory _channelFactory;
    private readonly RabbitMqOptions _options;

    public RabbitMqEventBus(IRabbitMqChannelFactory channelFactory, RabbitMqOptions options)
    {
        _channelFactory = channelFactory;
        _options = options;
    }

    public async Task PublishAsync<T>(EventEnvelope<T> envelope, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(envelope.Payload);
        await PublishRawAsync(
            envelope.EventId,
            envelope.EventType,
            payload,
            envelope.Metadata,
            ct);
    }

    public async Task PublishRawAsync(Guid eventId, string eventType, string payload, IDictionary<string, string> metadata, CancellationToken ct)
    {
        var channel = await _channelFactory.CreateAsync(ct);

        await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, durable: true);

        var props = new BasicProperties();
        props.MessageId = eventId.ToString();
        props.Headers = metadata.ToDictionary(x => x.Key, x => (object?)x.Value);
        props.Persistent = true;

        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(
            exchange: _options.Exchange,
            routingKey: eventType,
            mandatory: false,
            body: body,
            basicProperties: props,
            cancellationToken: ct);
    }

    public async Task SubscribeAsync<T>(string eventType, Func<EventEnvelope<T>, Task> handler, CancellationToken ct)
    {
        var channel = await _channelFactory.CreateAsync(ct);

        var queue = $"{eventType}.queue";
        var dlq = $"{eventType}.dlq";

        await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        await channel.QueueDeclareAsync(queue, durable: true, autoDelete: false, exclusive: false);
        await channel.QueueDeclareAsync(dlq, durable: true, autoDelete: false, exclusive: false);
        await channel.QueueBindAsync(queue: queue, exchange: _options.Exchange, routingKey: eventType);

        await RabbitMqRetryTopology.DeclareAsync(channel, _options.Exchange, eventType, 30_000);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());

                var envelope = new EventEnvelope<T>(
                    Guid.Parse(ea.BasicProperties.MessageId),
                    eventType,
                    ea.BasicProperties.Headers?.ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value?.ToString() ?? string.Empty) ?? new Dictionary<string, string>(),
                    JsonSerializer.Deserialize<T>(body)!
                );

                await handler(envelope);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                RabbitMqMetrics.MessagesConsumed.Inc();
            }
            catch
            {
                await channel.BasicRejectAsync(ea.DeliveryTag, requeue: false, ct);
                RabbitMqMetrics.MessagesFailed.Inc();
            }
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer, cancellationToken: ct);
    }
}
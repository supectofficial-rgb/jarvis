namespace OysterFx.Infra.EventBus.RabbitMqBroker.Retry;

using RabbitMQ.Client;
using System.Collections.Generic;

public static class RabbitMqRetryTopology
{
    public static async Task DeclareAsync(IChannel channel, string exchange, string routingKey, int delayMs)
    {
        var retryQueue = $"{routingKey}.retry";
        var mainQueue = $"{routingKey}.queue";

        await channel.QueueDeclareAsync(
            queue: retryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-message-ttl"] = delayMs,
                ["x-dead-letter-exchange"] = exchange,
                ["x-dead-letter-routing-key"] = routingKey
            }!);

        await channel.QueueDeclareAsync(
            queue: mainQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);
    }
}
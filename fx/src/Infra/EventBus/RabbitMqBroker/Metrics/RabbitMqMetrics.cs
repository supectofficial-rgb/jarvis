namespace OysterFx.Infra.EventBus.RabbitMqBroker.Metrics;

using Prometheus;

public static class RabbitMqMetrics
{
    public static readonly Counter MessagesPublished = Metrics.CreateCounter("rabbitmq_messages_published", "Published messages");
    public static readonly Counter MessagesConsumed = Metrics.CreateCounter("rabbitmq_messages_consumed", "Consumed messages");
    public static readonly Counter MessagesFailed = Metrics.CreateCounter("rabbitmq_messages_failed", "Failed messages");
}
namespace OysterFx.Infra.EventBus.RabbitMqBroker;

public sealed class EventBusProviderOptions
{
    public const string SectionName = "EventBus";
    public string Provider { get; init; } = "RabbitMq";
}

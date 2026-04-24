namespace OysterFx.Infra.EventBus.RabbitMqBroker;

public sealed class RabbitMqOptions
{
    public const string Key = "RabbitMqBroker";
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
    public string Exchange { get; init; } = "insurance.events";
    public int RetryLimit { get; init; } = 4;
}
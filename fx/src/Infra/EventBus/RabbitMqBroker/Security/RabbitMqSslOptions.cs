namespace OysterFx.Infra.EventBus.RabbitMqBroker.Security;

public sealed class RabbitMqSslOptions
{
    public bool Enabled { get; init; }
    public string ServerName { get; init; } = "";
    public string CertPath { get; init; } = "";
}
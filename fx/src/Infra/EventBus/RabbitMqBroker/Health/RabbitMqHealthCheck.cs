namespace OysterFx.Infra.EventBus.RabbitMqBroker.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using OysterFx.Infra.EventBus.RabbitMqBroker.Connections;
using System.Threading.Tasks;

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnection _connection;

    public RabbitMqHealthCheck(IRabbitMqConnection connection)
        => _connection = connection;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct)
    {
        return Task.FromResult(_connection.IsConnected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("RabbitMQ disconnected"));
    }
}
namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.RabbitMqBroker.Connections;
using OysterFx.Infra.EventBus.RabbitMqBroker.Health;
using RabbitMQ.Client;

public static class RabbitMqServiceDIExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Key));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value);

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var opt = sp.GetRequiredService<RabbitMqOptions>();
            return new ConnectionFactory
            {
                HostName = opt.Host,
                Port = opt.Port,
                UserName = opt.UserName,
                Password = opt.Password
            };
        });

        services.AddSingleton<IRabbitMqConnection, RabbitMqPersistentConnection>();
        services.AddSingleton<IRabbitMqChannelFactory, RabbitMqChannelFactory>();
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("rabbitmq");

        return services;
    }
}
namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using OysterFx.Infra.EventBus.RabbitMqBroker.Consumers;

public static class RabbitMqReceiveExtensions
{
    // Compatibility shim for legacy hosting style:
    // app.Services.ReceiveEventFromRabbitMqMessageBus(new KeyValuePair<string,string>(...));
    public static IServiceProvider ReceiveEventFromRabbitMqMessageBus(
        this IServiceProvider services,
        KeyValuePair<string, string> _)
    {
        ConsumerRegistrationGuard.EnsureRegistered(services);
        return services;
    }
}


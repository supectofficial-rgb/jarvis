namespace OysterFx.Infra.EventBus.RabbitMqBroker.Consumers;

using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.EventBus.Abstractions;
using System.Threading;

internal static class ConsumerRegistrationGuard
{
    private static int _isRegistered;

    public static void EnsureRegistered(IServiceProvider serviceProvider)
    {
        if (Interlocked.Exchange(ref _isRegistered, 1) == 1)
            return;

        using var scope = serviceProvider.CreateScope();
        var registrar = scope.ServiceProvider.GetService<IEventConsumerRegistrar>();
        registrar?.RegisterConsumers();
    }
}


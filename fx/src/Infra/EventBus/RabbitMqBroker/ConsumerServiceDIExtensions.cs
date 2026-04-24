namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;
using OysterFx.Infra.EventBus.RabbitMqBroker.Consumers;
using OysterFx.Infra.EventBus.RabbitMqBroker.Consumers.Convention;
using System.Reflection;

public static class ConsumerServiceDIExtensions
{
    public static IServiceCollection AddMessageConsumers<TRegistrar>(this IServiceCollection services)
        where TRegistrar : class, IEventConsumerRegistrar
    {
        services.AddSingleton<IEventConsumerRegistrar, TRegistrar>();
        services.AddHostedService<RabbitMqConsumerHostedService>();
        return services;
    }

    public static IServiceCollection AddConventionMessageConsumers(this IServiceCollection services, params Assembly[] assemblies)
    {
        var registrations = DiscoverRegistrations(assemblies).ToArray();

        foreach (var registration in registrations)
        {
            RegisterHandlersForPayload(services, assemblies, registration.PayloadType);
        }

        services.AddSingleton<IEventConsumerRegistrar>(sp =>
            new ConventionEventConsumerRegistrar(
                sp.GetRequiredService<IEventBus>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                registrations));

        services.AddHostedService<RabbitMqConsumerHostedService>();
        return services;
    }

    private static IEnumerable<EventConsumerRegistration> DiscoverRegistrations(IEnumerable<Assembly> assemblies)
    {
        var handlerInterface = typeof(IEventHandler<>);

        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsClass: true })
            .SelectMany(type => type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
                .Select(i => i.GetGenericArguments()[0]))
            .Distinct()
            .Select(payloadType => new EventConsumerRegistration(
                payloadType,
                IntegrationEventTypeResolver.Resolve(payloadType, explicitEventType: null)));
    }

    private static void RegisterHandlersForPayload(IServiceCollection services, IEnumerable<Assembly> assemblies, Type payloadType)
    {
        var contractType = typeof(IEventHandler<>).MakeGenericType(payloadType);

        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsClass: true })
            .Where(t => contractType.IsAssignableFrom(t))
            .Distinct();

        foreach (var handlerType in handlerTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(contractType, handlerType));
        }
    }
}

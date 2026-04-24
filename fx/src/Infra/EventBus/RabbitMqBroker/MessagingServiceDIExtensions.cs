namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Outbox;
using System.Reflection;

public static class MessagingServiceDIExtensions
{
    // Backward-compatible naming aligned with framework usage in microservices.
    public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddEventBusPublisher(configuration);
    }

    // Convention-based consumer registration; microservice only calls AddRabbitMqConsumer(configuration).
    public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic)
            .Where(x =>
            {
                var name = x.GetName().Name ?? string.Empty;
                return name.StartsWith("Insurance.", StringComparison.OrdinalIgnoreCase)
                    || name.StartsWith("OysterFx.", StringComparison.OrdinalIgnoreCase);
            })
            .ToArray();

        return services.AddEventBusConsumer(configuration, assemblies);
    }

    public static IServiceCollection AddEventBusPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguredEventBusBroker(configuration);
        return services;
    }

    public static IServiceCollection AddEventBusPublisherWithOutbox<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddConfiguredEventBusBroker(configuration);
        services.AddEventOutbox<TDbContext>(configuration);
        return services;
    }

    public static IServiceCollection AddEventBusConsumer<TRegistrar>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TRegistrar : class, IEventConsumerRegistrar
    {
        services.AddConfiguredEventBusBroker(configuration);
        services.AddMessageConsumers<TRegistrar>();
        return services;
    }

    public static IServiceCollection AddEventBusConsumer(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] handlerAssemblies)
    {
        services.AddConfiguredEventBusBroker(configuration);
        services.AddConventionMessageConsumers(handlerAssemblies);
        return services;
    }

    public static IServiceCollection AddEventBusPublisherAndConsumer<TDbContext, TRegistrar>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
        where TRegistrar : class, IEventConsumerRegistrar
    {
        services.AddConfiguredEventBusBroker(configuration);
        services.AddEventOutbox<TDbContext>(configuration);
        services.AddMessageConsumers<TRegistrar>();
        return services;
    }

    public static IServiceCollection AddEventBusPublisherAndConsumer<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] handlerAssemblies)
        where TDbContext : DbContext
    {
        services.AddConfiguredEventBusBroker(configuration);
        services.AddEventOutbox<TDbContext>(configuration);
        services.AddConventionMessageConsumers(handlerAssemblies);
        return services;
    }

    private static IServiceCollection AddConfiguredEventBusBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EventBusProviderOptions>(configuration.GetSection(EventBusProviderOptions.SectionName));

        var options = configuration.GetSection(EventBusProviderOptions.SectionName).Get<EventBusProviderOptions>()
            ?? new EventBusProviderOptions();

        var provider = options.Provider?.Trim();

        if (string.Equals(provider, "RabbitMq", StringComparison.OrdinalIgnoreCase))
        {
            services.AddRabbitMq(configuration);
            return services;
        }

        throw new NotSupportedException(
            $"EventBus provider '{provider}' is not supported in fx yet. Supported providers: RabbitMq.");
    }
}

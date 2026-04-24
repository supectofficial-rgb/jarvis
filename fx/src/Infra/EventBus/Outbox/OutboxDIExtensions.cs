namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;

public static class OutboxDIExtensions
{
    public static IServiceCollection AddEventOutbox<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddOptions<OutboxDispatcherOptions>();
        return AddEventOutboxCore<TDbContext>(services);
    }

    public static IServiceCollection AddEventOutbox<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = OutboxDispatcherOptions.SectionName)
        where TDbContext : DbContext
    {
        services.Configure<OutboxDispatcherOptions>(configuration.GetSection(sectionName));
        return AddEventOutboxCore<TDbContext>(services);
    }

    public static IServiceCollection AddEventOutbox<TDbContext>(
        this IServiceCollection services,
        Action<OutboxDispatcherOptions> setup)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(setup);
        services.Configure(setup);
        return AddEventOutboxCore<TDbContext>(services);
    }

    public static IServiceCollection AddDomainEventOutboxDispatcher<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DomainOutboxDispatcherOptions.SectionName)
        where TDbContext : DbContext
    {
        var configuredSection = configuration.GetSection(sectionName);

        // backward-compatible fallback with old framework section naming.
        if (!configuredSection.Exists() && string.Equals(sectionName, DomainOutboxDispatcherOptions.SectionName, StringComparison.Ordinal))
        {
            configuredSection = configuration.GetSection(OutboxDispatcherOptions.SectionName);
        }

        services.Configure<DomainOutboxDispatcherOptions>(configuredSection);
        return services.AddDomainEventOutboxDispatcher<TDbContext>();
    }

    public static IServiceCollection AddDomainEventOutboxDispatcher<TDbContext>(
        this IServiceCollection services,
        Action<DomainOutboxDispatcherOptions>? setup = null)
        where TDbContext : DbContext
    {
        if (setup is null)
            services.AddOptions<DomainOutboxDispatcherOptions>();
        else
            services.Configure(setup);

        services.AddScoped<IOutboxEventRepository, EfCoreOutboxEventRepository<TDbContext>>();
        services.AddHostedService<DomainOutboxDispatcherBackgroundService>();
        return services;
    }

    private static IServiceCollection AddEventOutboxCore<TDbContext>(IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IOutboxStore, EfCoreOutboxStore<TDbContext>>();
        services.AddScoped<IOutboxEventPublisher, OutboxEventPublisher>();
        services.AddHostedService<OutboxDispatcherBackgroundService>();
        return services;
    }
}

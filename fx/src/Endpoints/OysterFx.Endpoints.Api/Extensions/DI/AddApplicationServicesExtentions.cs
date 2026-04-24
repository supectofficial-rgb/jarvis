namespace OysterFx.Endpoints.Api.Extensions.DI;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.AppServices.Events;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Events;
using OysterFx.AppCore.Shared.Queries;
using System.Reflection;

public static class AddApplicationServicesExtensions
{
    public static IServiceCollection AddPearlFxApplicationServices(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddCommandHandlers(assembliesForSearch)
                    //.AddLogger(assembliesForSearch, PostFixConstants.CommandHandler)
                    .AddCommandBus()
                   .AddQueryHandlers(assembliesForSearch)
                    //.AddLogger(assembliesForSearch, PostFixConstants.QueryHandler)
                    .AddQueryBus()
                   .AddEventHandlers(assembliesForSearch)
                    .AddEventBus()
                   .AddFluentValidators(assembliesForSearch);

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddWithTransientLifetime(assembliesForSearch, typeof(ICommandHandler<>), typeof(ICommandHandler<,>));

    private static IServiceCollection AddCommandBus(this IServiceCollection services)
    {
        services.AddTransient<CommandBus>();

        services.AddTransient<ICommandBus>(provider =>
        {
            var inner = provider.GetRequiredService<CommandBus>();
            return new ValidatingCommandBusDecorator(inner, provider,
                provider.GetRequiredService<ILogger<ValidatingCommandBusDecorator>>());
        });

        return services;
    }

    private static IServiceCollection AddLogger(this IServiceCollection services, IEnumerable<Assembly> assemblies, string typePostFix)
    {
        foreach (var assembly in assemblies)
            services.AddLoggedServices(assembly, typePostFix);

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddWithTransientLifetime(assembliesForSearch, typeof(IQueryHandler<,>));

    private static IServiceCollection AddQueryBus(this IServiceCollection services)
    {
        services.AddTransient<QueryBus, QueryBus>();

        services.AddTransient<IQueryBus>(c =>
        {
            var queryDispatcher = c.GetRequiredService<QueryBus>();
            return queryDispatcher;
        });
        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddWithTransientLifetime(assembliesForSearch, typeof(IDomainEventHandler<>));

    private static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddTransient<EventBus, EventBus>();

        services.AddTransient<IEventBus>(provider =>
        {
            var eventBus = provider.GetRequiredService<EventBus>();
            return eventBus;
        });

        return services;
    }

    private static IServiceCollection AddFluentValidators(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddValidatorsFromAssemblies(assembliesForSearch);
}

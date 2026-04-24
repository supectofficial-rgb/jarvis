namespace OysterFx.Endpoints.Api.Extensions.DI;

using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Queries;
using System.Reflection;

public static class AddDataAccessExtentsions
{
    public static IServiceCollection AddOysterFxDataAccess(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddRepositories(assembliesForSearch);

    public static IServiceCollection AddRepositories(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddWithTransientLifetime(assembliesForSearch, typeof(ICommandRepository<,>), typeof(IQueryRepository));

    public static IServiceCollection AddOutboxEventRepositories(this IServiceCollection services)
    {
        // services.AddTransient<IOutboxEventRepository, OutboxEventRepository>();
        return services;
    }
}

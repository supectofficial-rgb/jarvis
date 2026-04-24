namespace Insurance.GraphService.Infra.InternalServices.Neo4j.Models;

using Insurance.GraphService.AppCore.Shared.Graphs.Services;
using Insurance.GraphService.Infra.InternalServices.Neo4j.ServiceCallers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceInjection
{
    public static IServiceCollection AddGraphNeo4jServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Neo4jOptions>(configuration.GetSection(Neo4jOptions.SectionName));
        services.AddSingleton<IGraphStoreService, Neo4jGraphStoreService>();
        return services;
    }
}

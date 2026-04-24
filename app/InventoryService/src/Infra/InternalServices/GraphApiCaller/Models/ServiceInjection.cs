namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models;

using Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceInjection
{
    public static IServiceCollection AddGraphProjectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GraphApiOptions>().Bind(configuration.GetSection(GraphApiOptions.Key));
        return services;
    }
}

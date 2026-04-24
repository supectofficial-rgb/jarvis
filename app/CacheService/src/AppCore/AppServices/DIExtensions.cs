namespace Insurance.CacheService.AppCore.AppServices;

using Insurance.CacheService.AppCore.AppServices.Cache.Services;
using Insurance.CacheService.AppCore.AppServices.Vector.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class DIExtensions
{
    public static IServiceCollection AddCacheServiceAppCoreServices(this IServiceCollection services)
    {
        services.TryAddScoped<ICacheApplicationService, CacheApplicationService>();
        services.TryAddScoped<IVectorApplicationService, VectorApplicationService>();
        return services;
    }
}

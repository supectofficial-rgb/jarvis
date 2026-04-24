namespace Insurance.CacheService.Infra.CallerService.Models;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Common;
using Insurance.CacheService.Infra.CallerService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.Auth.JwtServices;

public static class ServiceInjection
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CacheApiOptions>().Bind(configuration.GetSection(CacheApiOptions.Key));

        services.AddTransient<ICacheServiceCaller, CacheServiceCaller>();
        services.AddJwtGeneratorServices();
        services.AddHttpClient<HttpService>("CacheApiHttpService");
        return services;
    }
}
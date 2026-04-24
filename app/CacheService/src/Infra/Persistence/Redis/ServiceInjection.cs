namespace Insurance.CacheService.Infra.Persistence.Redis;

using Insurance.CacheService.AppCore.Shared.Cache.Services;
using Insurance.CacheService.AppCore.Shared.Configuration;
using Insurance.CacheService.AppCore.Shared.Vector.Services;
using Insurance.CacheService.Infra.Persistence.Redis.Models;
using Insurance.CacheService.Infra.Persistence.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

public static class ServiceInjection
{
    public static IServiceCollection AddCacheServiceRedisPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheStoreOptions>(configuration.GetSection(CacheStoreOptions.SectionName));
        services.Configure<VectorStoreOptions>(configuration.GetSection(VectorStoreOptions.SectionName));

        var connectionString = configuration.GetConnectionString(RedisOptions.ConnectionStringName)
            ?? throw new InvalidOperationException("Redis connection string is not configured.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "CacheApi";
        });

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddScoped<ICacheStoreService, RedisCacheStoreService>();
        services.AddScoped<IVectorStoreService, RedisVectorStoreService>();

        return services;
    }
}

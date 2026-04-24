namespace OysterFx.Endpoints.Api.Extensions.DI;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OysterFx.Endpoints.Api.Extensions.Proxies;
using System;
using System.Linq;
using System.Reflection;

public static class LoggerExtensions
{
    public static IServiceCollection AddLoggedServices(this IServiceCollection services, Assembly assembly, string typePostFix)
    {
        var interfaceTypes = assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Service"));

        foreach (var interfaceType in interfaceTypes)
        {
            var implementationType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && interfaceType.IsAssignableFrom(t));

            if (implementationType != null)
            {
                // Register with logging proxy
                services.AddTransient(interfaceType, provider =>
                {
                    var logger = provider.GetRequiredService<ILogger>();
                    var serviceInstance = ActivatorUtilities.CreateInstance(provider, implementationType);
                    return CreateProxy(interfaceType, serviceInstance, logger);
                });
            }
        }

        return services;
    }

    private static object CreateProxy(Type interfaceType, object serviceInstance, ILogger logger)
    {
        // Use reflection to invoke MethodLoggerProxy<T>.Create()
        var proxyType = typeof(MethodLoggerProxy<>).MakeGenericType(interfaceType);
        var createMethod = proxyType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
        return createMethod!.Invoke(null, new object[] { serviceInstance, logger })!;
    }
}
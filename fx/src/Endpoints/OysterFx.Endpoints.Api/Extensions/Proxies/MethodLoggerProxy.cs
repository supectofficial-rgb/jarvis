using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace OysterFx.Endpoints.Api.Extensions.Proxies;

public class MethodLoggerProxy<T> : DispatchProxy where T : class
{
    private T _decorated;
    private ILogger _logger;

    protected override object Invoke(MethodInfo method, object[] args)
    {
        var className = typeof(T).Name;
        var methodName = method.Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Log method start with parameters
            _logger.LogInformation(className, $"🚀 Starting {methodName}", ConvertToDictionary(method, args));

            var result = method.Invoke(_decorated, args);

            // Log method completion with duration
            _logger.LogInformation(className, $"✅ Completed {methodName} in {stopwatch.ElapsedMilliseconds}ms");
            return result;
        }
        catch (Exception ex)
        {
            // Log error with parameters
            _logger.LogError(className, $"❌ Failed {methodName} after {stopwatch.ElapsedMilliseconds}ms", ex, ConvertToDictionary(method, args));
            throw;
        }
    }

    private Dictionary<string, object> ConvertToDictionary(MethodInfo method, object[] args)
    {
        var dict = new Dictionary<string, object>();
        var parameters = method.GetParameters();

        for (int i = 0; i < parameters.Length; i++)
        {
            dict[parameters[i].Name] = args[i] ?? "null";
        }
        return dict;
    }

    public static T Create(T decorated, ILogger logger)
    {
        object proxy = Create<T, MethodLoggerProxy<T>>();
        ((MethodLoggerProxy<T>)proxy)._decorated = decorated;
        ((MethodLoggerProxy<T>)proxy)._logger = logger;
        return (T)proxy;
    }
}
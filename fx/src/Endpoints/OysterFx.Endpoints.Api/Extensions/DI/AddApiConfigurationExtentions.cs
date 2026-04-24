namespace OysterFx.Endpoints.Api.Extensions.DI;

using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OysterFx.Endpoints.Api.Middlewares.ApiExceptionHandler;

public static class AddApiConfigurationExtensions
{
    public static IServiceCollection AddOysterFxApiCore(this IServiceCollection services, params string[] assemblyNamesForLoad)
    {
        services.AddControllers().AddFluentValidation();
        services.AddSkyPearFxDependencies(assemblyNamesForLoad);

        return services;
    }

    public static void UseOysterFxApiExceptionHandler(this IApplicationBuilder app)
    {

        app.UseApiExceptionHandler(options =>
        {
            options.AddResponseDetails = (context, ex, error) =>
            {
                if (ex.GetType().Name == typeof(Microsoft.Data.SqlClient.SqlException).Name)
                {
                    error.Detail = "Exception was a database exception!";
                }
            };
            options.DetermineLogLevel = ex =>
            {
                if (ex.Message.StartsWith("cannot open database", StringComparison.InvariantCultureIgnoreCase) ||
                    ex.Message.StartsWith("a network-related", StringComparison.InvariantCultureIgnoreCase))
                {
                    return LogLevel.Critical;
                }
                return LogLevel.Error;
            };
        });

    }
}
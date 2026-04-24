namespace Insurance.CoreService.Endpoints.Api.Extensions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(connectionString: configuration
            .GetConnectionString("CommandDb")!, healthQuery: "SELECT 1;", name: "sqlserver", failureStatus: HealthStatus.Unhealthy, tags: new[] { "database", "sql", "Insurance Core Service" });
        return services;
    }
}
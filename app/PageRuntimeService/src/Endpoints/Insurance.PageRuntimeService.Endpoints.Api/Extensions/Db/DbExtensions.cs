namespace Insurance.PageRuntimeService.Endpoints.Api.Extensions.Db;

using Insurance.PageRuntimeService.Infra.Persistence.RDB.Commands;
using Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries;
using Microsoft.EntityFrameworkCore;

public static class DbExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = configuration.GetSection("ConnectionStrings").Get<DatabaseConnectionStrings>()
            ?? new DatabaseConnectionStrings();

        var commandDb = ResolveConnectionString(connectionStrings.CommandDb, connectionStrings.DefaultConnection, "CommandDb");
        var queryDb = ResolveConnectionString(connectionStrings.QueryDb, connectionStrings.DefaultConnection, "QueryDb");

        services.AddDbContext<PageRuntimeServiceCommandDbContext>(options =>
        {
            options.UseNpgsql(commandDb);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        services.AddDbContext<PageRuntimeServiceQueryDbContext>(options =>
        {
            options.UseNpgsql(queryDb);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        return services;
    }

    private static string ResolveConnectionString(string? primary, string? fallback, string name)
    {
        var value = string.IsNullOrWhiteSpace(primary) ? fallback : primary;
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Connection string '{name}' is not configured.");

        return value;
    }
}

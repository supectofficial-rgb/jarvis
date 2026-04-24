namespace Insurance.CoreService.Endpoints.Api.Extensions.Db;

using Insurance.Infra.Persistence.Sql.Commands;
using Insurance.Infra.Persistence.Sql.Queries;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands.Interceptors;

public static class DbExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration
        var connectionStrings = configuration.GetSection("ConnectionStrings").Get<DatabaseConnectionStrings>();

        // Register DbContext with provider switching
        services.AddDbContext<InsuranceCommandDbContext>(options =>
         {
             options.UseNpgsql(connectionStrings!.CommandDb);
             AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
             options.AddInterceptors(new AddAuditDataInterceptor());
         });

        services.AddDbContext<InsuranceQueryDbContext>(options =>
        {
            options.UseNpgsql(connectionStrings!.QueryDb);
        });

        return services;
    }
}
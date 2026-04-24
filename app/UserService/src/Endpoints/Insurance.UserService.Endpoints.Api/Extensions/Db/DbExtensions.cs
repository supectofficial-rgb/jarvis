namespace Insurance.UserService.Endpoints.Api.Extensions.Db;

using Insurance.UserService.Infra.Persistence.RDB.Queries;
using Microsoft.EntityFrameworkCore;

public static class DbExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration
        var connectionStrings = configuration.GetSection("ConnectionStrings").Get<DatabaseConnectionStrings>();

        // Register DbContext with provider switching
        //services.AddDbContext<InsuranceUserServiceAppDbContext>(options =>
        // {
        //     options.UseNpgsql(connectionStrings!.DefaultConnection);
        //     AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //     options.AddInterceptors(new AddAuditDataInterceptor());
        // });

        services.AddDbContext<InsuranceUserServiceQueryDbContext>(options =>
        {
            options.UseNpgsql(connectionStrings!.DefaultConnection);
        });

        return services;
    }
}
using Insurance.HubService.Endpoints.Api;
using Insurance.HubService.Endpoints.Api.Extensions.Db;
using Insurance.HubService.Infra.Persistence.RDB.Commands;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.ConfigureServices().ConfigurePipeline();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var commandDbContext = scope.ServiceProvider.GetRequiredService<HubServiceCommandDbContext>();
            commandDbContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("HubService.Startup");
            logger.LogError(ex, "An error occurred while initializing HubService database.");
        }
    }

    Log.Information("Hub Service API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Hub Service API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

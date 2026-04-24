using Insurance.PageRuntimeService.Endpoints.Api;
using Insurance.PageRuntimeService.Infra.Persistence.RDB.Commands;

var builder = WebApplication.CreateBuilder(args);
var app = builder.ConfigureServices().ConfigurePipeline();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var commandDbContext = scope.ServiceProvider.GetRequiredService<PageRuntimeServiceCommandDbContext>();
        commandDbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("PageRuntimeService.Startup");
        logger.LogError(ex, "An error occurred while initializing PageRuntimeService database.");
    }
}

app.Run();

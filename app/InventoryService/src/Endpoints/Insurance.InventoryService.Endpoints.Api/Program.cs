using Insurance.InventoryService.Endpoints.Api;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands;

var builder = WebApplication.CreateBuilder(args);
var app = builder.ConfigureServices().ConfigurePipeline();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var commandDbContext = scope.ServiceProvider.GetRequiredService<InventoryServiceCommandDbContext>();
        commandDbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("InventoryService.Startup");
        logger.LogError(ex, "An error occurred while initializing InventoryService database.");
    }
}

app.Run();

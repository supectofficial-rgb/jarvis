using Insurance.CoreService.Endpoints.Api;
using Insurance.Infra.Persistence.Sql.Commands;

var builder = WebApplication.CreateBuilder(args);
var app = builder.ConfigureServices().ConfigurePipeline();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<InsuranceCommandDbContext>();
        context.Database.EnsureCreated(); // Creates database and tables if they don't exist
        // Or use: context.Database.Migrate(); // Applies any pending migrations
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();
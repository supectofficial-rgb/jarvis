using Insurance.AiAssistService.Endpoints.Api;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.ConfigureServices().ConfigurePipeline();

    Log.Information("AiAssist Service API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AiAssist Service API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

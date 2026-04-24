namespace Insurance.CacheService.Endpoints.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.ConfigureServices();
        app.ConfigurePipeline();
        app.Run();
    }
}

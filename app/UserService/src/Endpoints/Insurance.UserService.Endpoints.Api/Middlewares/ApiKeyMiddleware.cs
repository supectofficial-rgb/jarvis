namespace Insurance.UserService.Endpoints.Api.Middlewares;

public class ApiKeyMiddleware : IMiddleware
{
    private readonly IConfiguration _config;

    public ApiKeyMiddleware(IConfiguration config)
    {
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Skip swagger and health endpoints
        if (path!.StartsWith("/swagger") || path.StartsWith("/favicon.ico"))
        {
            await next(context);
            return;
        }

        var validKey = _config["ApiKeys:Main"];
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        if (!string.Equals(extractedKey, validKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API Key.");
            return;
        }

        await next(context);
    }
}

namespace Insurance.CoreService.Endpoints.Api.Extensions;

using Microsoft.AspNetCore.RateLimiting;

public static class RateLimitExtensions
{
    public static IServiceCollection AddRateLimitServices(this IServiceCollection services, IConfiguration configuration)
    {

        // Register middleware
        // services.AddTransient<ApiKeyMiddleware>();

        // Add Rate Limiting (Token Bucket or Sliding Window)
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", limiterOptions =>
            {
                limiterOptions.PermitLimit = 60;               // 10 requests
                limiterOptions.Window = TimeSpan.FromSeconds(10); // per 10 seconds
                limiterOptions.QueueLimit = 0;                // do not queue
                limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            });
        });
        return services;
    }
}
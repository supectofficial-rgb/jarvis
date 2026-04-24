namespace OysterFx.Infra.Auth.JwtServices;

using Microsoft.AspNetCore.Builder;

public static class JwtTokenProcessingMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenProcessing(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtTokenProcessingMiddleware>();
    }
}
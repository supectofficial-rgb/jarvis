namespace OysterFx.Infra.Auth.JwtServices;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

public static class JwtAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthAuthz(this IApplicationBuilder app, IConfiguration configuration)
    {
        var enableJwtAuth = bool.Parse(configuration["Jwt:Enabled"]!);
        if (enableJwtAuth)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        return app;
    }
}
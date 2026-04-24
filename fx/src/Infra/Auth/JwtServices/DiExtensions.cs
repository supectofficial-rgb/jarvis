namespace OysterFx.Infra.Auth.JwtServices;

using Microsoft.Extensions.DependencyInjection;

public static class DiExtensions
{
    public static IServiceCollection AddJwtGeneratorServices(this IServiceCollection services)
    {
        services.AddTransient<IGenerateDafaultJwtTokenService, GenerateDafaultJwtTokenService>();
        return services;
    }
}
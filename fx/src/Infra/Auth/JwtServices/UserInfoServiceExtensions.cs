namespace OysterFx.Infra.Auth.JwtServices;

using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.Auth.UserServices;

public static class UserInfoServiceExtensions
{
    public static IServiceCollection AddJwtUserInfoService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        return services.AddScoped<IUserInfoService, JwtUserInfoService>();
    }
}
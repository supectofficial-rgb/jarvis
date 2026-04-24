namespace Insurance.Infra.InternalServices.UserApiCaller.Models;

using Insurance.Infra.InternalServices.UserApiCaller.Abstractions;
using Insurance.Infra.InternalServices.UserApiCaller.Models.Constants;
using Insurance.Infra.InternalServices.UserApiCaller.ServiceCallers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.Auth.JwtServices;
using OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public static class ServiceInjection
{
    public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<UserServiceOptions>().Bind(configuration.GetSection(UserServiceOptions.Key));
        services.AddTransient<IUserService, UserService>();
        services.AddJwtGeneratorServices();
        services.AddHttpService();
        return services;
    }
}
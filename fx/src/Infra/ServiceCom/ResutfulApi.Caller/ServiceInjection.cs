using Microsoft.Extensions.DependencyInjection;

namespace OysterFx.Infra.ServiceCom.ResutfulApi.Caller;
public static class ServiceInjection
{
    public static void AddHttpService(this IServiceCollection services)
    {
        services.AddHttpClient<HttpService>();
    }
}
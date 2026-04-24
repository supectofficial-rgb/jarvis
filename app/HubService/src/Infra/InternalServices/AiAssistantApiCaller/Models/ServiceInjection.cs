namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models;

using Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public static class ServiceInjection
{
    public static IServiceCollection AddAiAssistantServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AiAssistantApiOptions>().Bind(configuration.GetSection(AiAssistantApiOptions.Key));
        services.AddHttpService();
        return services;
    }
}

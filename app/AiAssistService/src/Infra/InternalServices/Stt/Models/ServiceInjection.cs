namespace Insurance.AiAssistService.Infra.InternalServices.STT;

using Insurance.AiAssistService.AppCore.Shared.Conversations.Services;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceInjection
{
    public static IServiceCollection AddAiAssistSttServices(this IServiceCollection services)
    {
        services.AddScoped<ISttService, WhisperService>();
        return services;
    }
}


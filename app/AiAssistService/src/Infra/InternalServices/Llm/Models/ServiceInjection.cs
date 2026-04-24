namespace Insurance.AiAssistService.Infra.InternalServices.LLM;

using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceInjection
{
    public static IServiceCollection AddAiAssistLlmServices(this IServiceCollection services)
    {
        services.AddHttpClient<IIntentLlmService, IntentLlmService>();
        return services;
    }
}


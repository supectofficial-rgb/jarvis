using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;

public sealed class ParameterResolver : IParameterResolver, ISingletoneLifetimeMarker
{
    public Dictionary<string, string?> Merge(IntentResolutionResult intent, AssistantSession session)
    {
        var merged = new Dictionary<string, string?>(session.CollectedParams, StringComparer.OrdinalIgnoreCase);

        foreach (var param in intent.Parameters)
        {
            if (!string.IsNullOrWhiteSpace(param.Value))
            {
                merged[param.Key] = param.Value;
            }
        }

        return merged;
    }
}





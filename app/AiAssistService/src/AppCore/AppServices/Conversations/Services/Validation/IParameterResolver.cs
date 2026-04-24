using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;

public interface IParameterResolver
{
    Dictionary<string, string?> Merge(IntentResolutionResult intent, AssistantSession session);
}



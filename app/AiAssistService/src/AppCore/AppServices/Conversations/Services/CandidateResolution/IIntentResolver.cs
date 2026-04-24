using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public interface IIntentResolver
{
    IntentResolutionResult Resolve(IReadOnlyList<ActionCandidate> candidates, LlmIntentResult llmResult);
}



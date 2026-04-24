using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Prompting;

public interface IPromptBuilder
{
    LlmPrompt Build(string normalizedText, AssistantSession session, IReadOnlyList<ActionCandidate> candidates, IReadOnlyList<ActionMetadata> catalog);
}


